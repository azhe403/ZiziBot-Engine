﻿using Microsoft.EntityFrameworkCore;
using Serilog;
using ZiziBot.Database.MongoDb.Entities;

namespace ZiziBot.Application.Handlers.Telegram.Rss;

public class AddRssRequest : BotRequestBase
{
}

public class AddRssHandler(
    DataFacade dataFacade,
    ServiceFacade serviceFacade
)
    : IBotRequestHandler<AddRssRequest>
{
    public async Task<BotResponseBase> Handle(AddRssRequest request, CancellationToken cancellationToken)
    {
        serviceFacade.TelegramService.SetupResponse(request);

        if (!await dataFacade.FeatureFlag.IsEnabled(Flag.RSS_BROADCASTER))
            return await serviceFacade.TelegramService.SendMessageAsync("Fitur RSS saat ini sedang dimatikan");

        if (request.Param.IsNullOrEmpty())
            return await serviceFacade.TelegramService.SendMessageAsync("Masukkan RSS URL yang ingin ditambahkan");

        try
        {
            var rssUrl = await request.Param.DetectRss();
            await serviceFacade.TelegramService.SendMessageAsync("Sedang memverifikasi URL..");

            var rssSetting = await dataFacade.MongoDb.RssSetting
                .Where(entity => entity.RssUrl == rssUrl)
                .Where(entity => entity.ChatId == request.ChatIdentifier)
                .Where(entity => entity.ThreadId == request.MessageThreadId)
                .Where(entity => entity.Status == EventStatus.Complete)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            if (rssSetting != null)
                return await serviceFacade.TelegramService.SendMessageAsync("RSS Sudah disimpan");

            var uniqueId = await StringUtil.GenerateRssKeyAsync();

            var create = dataFacade.MongoDb.RssSetting.Add(new RssSettingEntity {
                ChatId = request.ChatIdentifier,
                RssUrl = rssUrl,
                ThreadId = request.MessageThreadId,
                UserId = request.UserId,
                CronJobId = uniqueId,
                Status = EventStatus.Complete
            });

            await dataFacade.MongoDb.SaveChangesAsync(cancellationToken);

            await serviceFacade.TelegramService.SendMessageAsync("Membuat Cron Job..");

            await serviceFacade.JobService.Register(request.ChatIdentifier, request.MessageThreadId, rssUrl, uniqueId, true);

            return await serviceFacade.TelegramService.SendMessageAsync("RSS Berhasil disimpan" +
                                                                        "\nURL: " +
                                                                        rssUrl);
        }
        catch (Exception e)
        {
            Log.Error(e, "Failed when adding RSS Url: {Url} to ChatId: {ChatId}", request.Param, request.ChatIdentifier);
            return await serviceFacade.TelegramService.SendMessageAsync("Terjadi sesuatu ketika menambahkan RSS");
        }
    }
}