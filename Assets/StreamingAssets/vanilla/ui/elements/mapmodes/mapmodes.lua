local transform;

onClicks = {
    { "Texture", "HandleTextureModeClicked" },
    { "Ownership", "HandleOwnershipModeClicked" }
};

onHovers = {
    { "Texture", "HandleTextureModeHovered" },
    { "Ownership", "HandleOwnershipModeHovered" }
};

local mapModes = {};

function OnScriptLoaded()
end

function OnScriptSet(_transform)
    transform = _transform;
    transform.SetSiblingIndex(0);

    mapModes = {
        transform.Find("Texture").gameObject.GetComponent("Outline"),
        transform.Find("Ownership").gameObject.GetComponent("Outline")
    };

    MapManager.OnMapModeChanged.add(HandleMapModeChanged);
end

function HandleMapModeChanged(sender, data)
    for i=1, #mapModes do
        mapModes[i].enabled = (i-1 == data.Mode);
    end
end

function HandleTextureModeClicked()
    MapManager.SetMapMode(0);
end

function HandleOwnershipModeClicked()
    MapManager.SetMapMode(1);
end

function HandleTextureModeHovered()
    return {
        header = LanguageManager.Translate("TEXTURE_MODE_HOVER_TITLE"),
        description = "\n"..LanguageManager.Translate("TEXTURE_MODE_HOVER_DESCRIPTION")
    };
end

function HandleOwnershipModeHovered()
    return {
        header = LanguageManager.Translate("OWNERSHIP_MODE_HOVER_TITLE"),
        description = "\n"..LanguageManager.Translate("OWNERSHIP_MODE_HOVER_DESCRIPTION")
    };
end

