namespace Danfoss_Heat_Distribution_Optimizer.Services.Interfaces
{
    public interface IGraphicalUnit
    {
        string ImagePath {get; set;}
        string GetImagePath();
        void SetImagePath(int imagePath);
    }
}