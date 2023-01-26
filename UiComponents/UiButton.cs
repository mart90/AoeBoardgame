using ImGuiNET;
using System;
using System.Numerics;

namespace AoeBoardgame
{
    static class UiButton
    {
        public static void Create(TextureLibrary _textureLibrary, UiType uiType, Vector2 location, Action isClicked)
        {
            UiTexture button = _textureLibrary.GetUiTextureByType(uiType);

            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0, 0, 0, 0));
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0, 0, 0, 0));
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0, 0, 0, 0));
            ImGui.SetCursorScreenPos(location);
            ImGui.ImageButton(_textureLibrary.GetIntPtrByUiType(uiType), new Vector2(button.Texture.Width, button.Texture.Height));

            if (ImGui.IsItemClicked())
            {
                isClicked();
            }

            ImGui.PopStyleColor(3);
        }
    }
}
