using Microsoft.Xna.Framework;

namespace Ebilkill.Gui.Events
{
    public class ClickEvent
    {
        Vector2 position;

        public ClickEvent(Vector2 position)
        {
            this.position = position;
        }

        public Vector2 Position => position;
    }
}
