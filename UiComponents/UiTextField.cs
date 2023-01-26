using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoeBoardgame
{
    static class UiTextField
    {
        public static bool Create(float inputWidth = 300f, float inputHeight = 33f, System.Numerics.Vector2 location)
        {
            ImGui.GetWindowDrawList().AddImage(_textureLibrary.TextureToIntPtr(_textureLibrary.GetUiTextureByType(UiType.InputText)), new System.Numerics.Vector2(locationX, locationY), new System.Numerics.Vector2(locationX + inputWidth, locationY + inputHeight));
            ImGui.SetCursorScreenPos(new System.Numerics.Vector2(locationX, locationY));
            ImGui.SetNextItemWidth(inputWidth);
            ImGui.PushStyleColor(ImGuiCol.FrameBg, new System.Numerics.Vector4(0, 0, 0, 0));
            ImGui.PushStyleColor(ImGuiCol.Text, new System.Numerics.Vector4(0.8f, 0.7f, 0.5f, 1));
            var textField = ImGui.InputText("##username", _usernameBuffer, (uint)_usernameBuffer.Length, ImGuiInputTextFlags.AutoSelectAll);
            ImGui.PopStyleColor(2);

            return textField;
        }
    }
}
