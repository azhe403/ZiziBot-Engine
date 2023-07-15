namespace ZiziBot.Console.Pages
{
    public partial class NoteAdd : ReactiveInjectableComponentBase<NoteAddViewModel>
    {
        [Parameter]
        public string? NoteId { get; set; }

        [Inject]
        protected IMediator Mediator { get; set; }

        [Inject]
        protected ChatSettingRepository ChatSettingRepository { get; set; }

        public NoteAdd() => this.WhenActivated(x => { });

        protected override async Task OnParametersSetAsync()
        {
            if (NoteId != null)
            {
                var noteInfo = await ChatSettingRepository.GetNote(NoteId);

                ViewModel.NoteId = noteInfo.Id;
                ViewModel.ChatId = noteInfo.ChatId;
                ViewModel.Query = noteInfo.Query;
                ViewModel.Content = noteInfo.Text;
                ViewModel.RawButton = noteInfo.RawButton;
                ViewModel.FileId = noteInfo.FileId;
                ViewModel.MediaTypeId = noteInfo.DataType;
                ViewModel.CreatedDate = noteInfo.CreatedDate;
                ViewModel.UpdatedDate = noteInfo.UpdatedDate;
            }

            await base.OnParametersSetAsync();
        }
    }
}
