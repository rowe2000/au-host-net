using System.Linq;
using Xamarin.Forms;

namespace AuHost
{
    public class FrameView : ListView
    {
    }


    public class RackView : StackLayout
    {
        protected override void OnChildAdded(Element child)
        {
            base.OnChildAdded(child);
            HeightRequest = Children.Max(o => o.Y);
        }

        protected override void OnChildRemoved(Element child, int oldLogicalIndex)
        {
            base.OnChildRemoved(child, oldLogicalIndex);
            HeightRequest = Children.Max(o => o.Y);
        } 
    }
}