local transform

local topbar = {
    transform = nil,
    districtNameText = nil
}

local summary = {
    transform = nil,
    properties = {
        properties = {
            text = nil
        },
        population = {
            text = nil
        }
    },
    modifiers = {
        transform = nil,
        text = nil,
    }
}

local properties = {
    transform = nil,
    text = nil,
    property_prefab = nil,
    headers = {
        name = nil,
        condition = nil,
        age = nil,
        shares = nil,
        value = nil
    },
    content = nil
}

onClicks = {
    { "Topbar/CloseButton", "HandleCloseButtonPressed" }
}

onHovers = {
    { "Summary/Properties/Properties", "HandlePropertiesHovered" },
    { "Summary/Properties/Population", "HandlePopulationHovered" }
}

function OnScriptLoaded()
end

function OnScriptSet(_transform)
    transform = _transform

    GetTopbarElements(transform)
    GetSummaryElements(transform)
    GetPropertiesElements(transform)

    SetHandlers()

    transform.gameObject.SetActive(false)
end

function OnClickEventsSet()
end

function OnHoverEventsSet()
end

function HandleCloseButtonPressed()
    transform.gameObject.SetActive(false)
end

function HandleDistrictClicked(sender, district)
    -- Converting Map.District to District.District
    local district = DistrictDictionary.SafeGet(district.district_tag)

    topbar.districtNameText.text = district.Data.name

    summary.properties.properties.text.text = district.Data.size
    summary.properties.population.text.text = ""..district.population

    transform.gameObject.SetActive(true)
end

function SetHandlers()
    MapManager.OnDistrictClicked.add(HandleDistrictClicked)
end

function GetTopbarElements(parent)
    topbar.transform = parent.Find("Topbar")
    topbar.districtNameText = topbar.transform.Find("DistrictNameText").GetComponent("TextMeshProUGUI")
end

function GetSummaryElements(parent)
    summary.transform = parent.Find("Summary")

    summary.properties.properties.text = summary.transform.Find("Properties/Properties/Text").GetComponent("TextMeshProUGUI")
    summary.properties.population.text = summary.transform.Find("Properties/Population/Text").GetComponent("TextMeshProUGUI")

    summary.modifiers.transform = summary.transform.Find("Flags")
    summary.modifiers.text = summary.transform.Find("Flags/Text").GetComponent("TextMeshProUGUI")
end

function GetPropertiesElements(parent)
    properties.transform = parent.Find("Properties")
    properties.property_prefab = properties.transform.Find("PropertyElement")

    properties.text = properties.transform.Find("Text")

    properties.headers = {
        name = properties.transform.Find("Headers/Name"),
        condition = properties.transform.Find("Headers/Condition"),
        age = properties.transform.Find("Headers/Age"),
        shares = properties.transform.Find("Headers/Shares"),
        value = properties.transform.Find("Headers/Value"),
    }

    properties.content = properties.transform.Find("Scroll View/Viewport/Content")
end

function HandlePropertiesHovered()
    return {
        header = "District Size",
        description = "Max amount of properties this district can hold.",
    }
end

function HandlePopulationHovered()
    return {
        header = "Population",
        description = "Population of this district.",
    }
end
