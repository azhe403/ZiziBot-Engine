using System.Collections;

namespace ZiziBot.Console.Partials;

public partial class MediaTypeSelector
{
    [Parameter]
    public int MediaTypeId { get; set; }

    [Parameter]
    public EventCallback<int> MediaTypeIdChanged { get; set; }

    public IEnumerable<MediaType> ListChat { get; set; }

    private void OnLoadData(LoadDataArgs obj)
    {
        ListChat = new List<MediaType>()
        {
            new()
            {
                Id = 1,
                Name = "Text"
            },
            new()
            {
                Id = 2,
                Name = "Photo"
            },
            new()
            {
                Id = 3,
                Name = "Audio"
            },
            new()
            {
                Id = 4,
                Name = "Video"
            },
            new()
            {
                Id = 6,
                Name = "Document"
            },
            new()
            {
                Id = 7,
                Name = "Sticker"
            }
        };
    }

    private async Task ValueChanged(object obj)
    {
        if (obj is not int mediaTypeId) return;

        await MediaTypeIdChanged.InvokeAsync(mediaTypeId);
        await InvokeAsync(StateHasChanged);
    }
}

public class MediaType
{
    public int Id { get; set; }
    public string Name { get; set; }
}