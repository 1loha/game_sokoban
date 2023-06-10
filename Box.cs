using System.Windows.Forms; // для работы с label

namespace SokobanKR
{
    class Box
    {
        private readonly static int len = 50; // все одного размера (шаг, коробка, игрок, стенка)
        
        public static bool RightBoxMove(Label player, Label[] box)
        {
            for (int i = 0; i < box.Length; i++)
            {
                int bX = box[i].Location.X;
                int bY = box[i].Location.Y;
                int pX = player.Location.X;
                int pY = player.Location.Y;

                if (pY == bY && pX == bX - len)
                {
                    if (RightSecondBox(player, box) ||
                        GameForm.CheckWall("Right", box[i]))
                            return false;

                    box[i].Left += len;
                    return true;
                    
                }
            }
            return true;
        }
        public static bool RightSecondBox(Label player, Label[] box)
        {
            for (int i = 0; i < box.Length; i++)
                if (player.Location.X == box[i].Location.X - len - len &&
                    player.Location.Y == box[i].Location.Y)
                    return true;
            return false;
        }

        public static bool LeftBoxMove(Label player, Label[] box)
        {
            for (int i = 0; i < box.Length; i++)
            {
                int bX = box[i].Location.X;
                int bY = box[i].Location.Y;
                int pX = player.Location.X;
                int pY = player.Location.Y;

                if (pY == bY && pX == bX + len)
                {
                    if (LeftSecondBox(player, box) ||
                        GameForm.CheckWall("Left", box[i]))
                            return false;
                        
                    box[i].Left -= len;
                    return true;
                }
            }
            return true;
        }
        public static bool LeftSecondBox(Label player, Label[] box)
        {
            for (int i = 0; i < box.Length; i++)
                if (player.Location.X == box[i].Location.X + len + len &&
                    player.Location.Y == box[i].Location.Y)
                    return true;
            return false;
        }

        public static bool UpBoxMove(Label player, Label[] box)
        {
            for (int i = 0; i < box.Length; i++)
            {
                int bX = box[i].Location.X;
                int bY = box[i].Location.Y;
                int pX = player.Location.X;
                int pY = player.Location.Y;

                if (pX == bX && pY == bY + len)
                {
                    if (UpSecondBox(player, box) ||
                        GameForm.CheckWall("Up", box[i]))
                            return false;

                    box[i].Top -= len;
                    return true;
                }
            }
            return true;
        }
        public static bool UpSecondBox(Label player, Label[] box)
        {
            for (int i = 0; i < box.Length; i++)
                if (player.Location.Y == box[i].Location.Y + len + len &&
                    player.Location.X == box[i].Location.X)
                    return true;
            return false;
        }

        public static bool DownBoxMove(Label player, Label[] box)
        {
            for (int i = 0; i < box.Length; i++)
            {
                int bX = box[i].Location.X;
                int bY = box[i].Location.Y;
                int pX = player.Location.X;
                int pY = player.Location.Y;

                if (pX == bX && pY == bY - len)
                {
                    if (DownSecondBox(player, box) ||
                        GameForm.CheckWall("Down", box[i]))
                            return false;
                    
                    box[i].Top += len;
                    return true;
                }
            }
            return true;
        }
        public static bool DownSecondBox(Label player, Label[] box)
        {
            for (int i = 0; i < box.Length; i++)
                if (player.Location.Y == box[i].Location.Y - len - len && 
                    player.Location.X == box[i].Location.X)
                    return true;
            return false;
        }


        internal static bool Checkwin(Label[] boxes, Label[] winPositions)
        {
            bool flag = false;
            
            for(int i = 0; i < winPositions.Length; i++)
            {
                if (winPositions[i] == null)
                    return flag;
                for (int j = 0; j < boxes.Length; j++)
                {
                    if (boxes[j] == null)
                        return flag;
                    flag = false;
                    if (winPositions[i].Bounds.IntersectsWith(boxes[j].Bounds))
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                    return false;
            }
            
            return flag;
        }
    }
}
