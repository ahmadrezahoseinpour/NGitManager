using GitManager.Dto.Epic;
using GitManager.Interface;
using Microsoft.AspNetCore.Components;

namespace GitUI.Components.Pages
{
    public partial class Epics
    {
        [Parameter] public int groupid { get; set; }
        [Parameter] public int epicIid { get; set; }


        public List<EpicDto> EpicsList { get; set; } = new List<EpicDto>();
        private EpicDto Epic { get; set; }
        private EpicDto UpdateDto { get; set; }
        private EpicDto AddDto { get; set; }

        private string Title { get; set; }
        private string Description { get; set; }

        protected override async Task OnInitializedAsync()
        {
            //EpicDto = await EpicService.GetAll(groupid);


            ////for Update
            //Epic = await EpicService.Get(groupid, EpicIid);
            //Title = epic.Title;
            //Description = epic.Description;


            ////for Get
            //Epic = await EpicService.Get(groupid, EpicIid);
        }

        private async Task CloseEpic(EpicDto dto)
        {
            await EpicService.Close(dto);
            EpicQueryDto eQueryDto = new();
            EpicsList = await EpicService.Search(groupid, eQueryDto);
            StateHasChanged();
        }

        private async Task OpenEpic(EpicDto dto)
        {
            await EpicService.Open(dto);
            EpicQueryDto eQueryDto = new();
            EpicsList = await EpicService.Search(groupid, eQueryDto);
            StateHasChanged();
        }

        private async Task CreateEpic()
        {
            var newepic = await EpicService.Create(AddDto);
            //NavigationManager.NavigateTo($"/epics/{groupid}/{newepic.EpicId}");
        }

        private async Task UpdateEpic()
        {
            var updatedepic = await EpicService.Update(UpdateDto);
            //NavigationManager.NavigateTo($"/epics/{groupid}/{updatedepic.EpicId}");
        }


    }
}
