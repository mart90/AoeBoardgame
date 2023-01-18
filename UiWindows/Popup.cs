using ImGuiNET;
using System;

namespace AoeBoardgame
{
    class Popup
    {
        /// <summary>
        /// Max length ~120 for now, or it will go out of bounds
        /// </summary>
        public string Message { get; set; }

        public bool IsInformational { get; set; }

        /// <summary>
        /// Up to the instantiator to define what happens if the user presses "Yes"
        /// </summary>
        public Action ActionOnConfirm { get; set; }
        
        public bool IsInteractedWith { get; set; }

        public void Draw()
        {
            string message = Message;

            if (message.Length > 60 && !message.Substring(0, 60).Contains("\n"))
            {
                for (int i = 0; i < 50; i++)
                {
                    if (message[59 - i] == ' ')
                    {
                        message = message.Remove(59 - i, 1);
                        message = message.Insert(59 - i, "\n");
                        break;
                    }
                }
            }

            ImGui.Text(message);
            ImGui.NewLine();

            if (!IsInformational)
            {
                if (ImGui.Button("Yes", new System.Numerics.Vector2(50, 20)))
                {
                    ActionOnConfirm();
                    IsInteractedWith = true;
                }
                ImGui.SameLine();
                if (ImGui.Button("No", new System.Numerics.Vector2(50, 20)))
                {
                    IsInteractedWith = true;
                }
            }
            else
            {
                if (ImGui.Button("Ok", new System.Numerics.Vector2(50, 20)))
                {
                    IsInteractedWith = true;
                }
            }
        }
    }
}
