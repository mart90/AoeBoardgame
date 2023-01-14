using ImGuiNET;
using System;

namespace AoeBoardgame
{
    class Popup
    {
        public string Message { get; set; }
        public bool IsInformational { get; set; }

        /// <summary>
        /// Up to the instantiator to define what happens if the user presses "Yes"
        /// </summary>
        public Action ActionOnConfirm { get; set; }
        
        public bool IsInteractedWith { get; set; }

        public void Draw()
        {
            ImGui.Text(Message);
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
                if (ImGui.Button("Ok"))
                {
                    IsInteractedWith = true;
                }
            }
        }
    }
}
