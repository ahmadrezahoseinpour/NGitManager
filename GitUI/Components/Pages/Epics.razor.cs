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
        private EpicDto epic { get; set; }

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

        private async Task CloseEpic(int epicIid)
        {
            await EpicService.Close(groupid, epicIid);
            EpicQueryDto eQDto = new();
            EpicsList = await EpicService.GetAll(groupid, eQDto);
            StateHasChanged();
        }

        private async Task OpenEpic(int epicIid)
        {
            await EpicService.Open(groupid, epicIid);
            EpicQueryDto eQDto = new();
            EpicsList = await EpicService.GetAll(groupid, eQDto);
            StateHasChanged();
        }

        private async Task CreateEpic()
        {
            var newepic = await EpicService.Create(groupid, Title, Description);
            NavigationManager.NavigateTo($"/epics/{groupid}/{newepic.EpicIid}");
        }

        private async Task UpdateEpic()
        {
            var updatedepic = await EpicService.Update(groupid, epicIid, title: Title, description: Description);
            NavigationManager.NavigateTo($"/epics/{groupid}/{updatedepic.EpicIid}");
        }


    }
}
