using Abp.Application.Services.Dto;

namespace JK.Web.Models.Common.Modals
{
    public class ModalHeaderViewModel
    {
        public string Title { get; set; }

        public ModalHeaderViewModel(string title)
        {
            Title = title;
        }
        public ModalHeaderViewModel(string title, IEntityDto<int> dto)
        {
            Title = $"{(dto.Id > 0 ? "编辑" : "新增")}{title}";
        }
        public ModalHeaderViewModel(string title, IEntityDto<long> dto)
        {
            Title = $"{(dto.Id > 0 ? "编辑" : "新增")}{title}";
        }
    }

}
