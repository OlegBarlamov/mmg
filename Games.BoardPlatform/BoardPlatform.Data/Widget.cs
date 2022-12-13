namespace BoardPlatform.Data
{
    public class Widget : IWidget
    {
        public IPosition2D GetPosition()
        {
            
        }

        public ISize2D GetSize()
        {
            
        }

        public IToken GetParentBoardId()
        {
            
        }

        public IToken GetToken()
        {
            throw new System.NotImplementedException();
        }

        public IRawWidgetData GetRawData()
        {
            throw new System.NotImplementedException();
        }

        public static IWidget FromRawWidgetData(IRawWidgetData rawWidgetData)
        {
            
        } 
    }
}