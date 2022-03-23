local transform
local element

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
    content = nil,
    elements = {}
}

local property_element_amount = 30

onClicks = {
    { "", "HandleMenuClicked" },
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
    element = transform.GetComponent("Element")

    GetTopbarElements(transform)
    GetSummaryElements(transform)
    GetPropertiesElements(transform)

    SetHandlers()
    CreatePropertyElements(property_element_amount)

    transform.gameObject.SetActive(false)
end

function OnClickEventsSet()
end

function OnHoverEventsSet()
end

function OnActivate(param)
end

function OnDeactivate(param)
    MapManager.HideActiveDistrict()
end

function HandleMenuClicked()
    UIManager.BringToFront(element)
end

function HandleCloseButtonPressed()
    transform.gameObject.SetActive(false)
    OnDeactivate()
end

function HandleDistrictClicked(sender, district)
    -- Converting Map.District to District.District
    local district = DistrictDictionary.SafeGet(district.district_tag)

    topbar.districtNameText.text = district.Data.name

    summary.properties.properties.text.text = district.Data.size
    summary.properties.population.text.text = ""..district.population

    UpdatePropertyElements(district.properties)
    UIManager.OpenMenu("GameMenu/district/")
end

function SetHandlers()
    MapManager.OnDistrictClicked.add(HandleDistrictClicked)
end

function UpdatePropertyElements(propertyList)
    if #properties.elements < #propertyList then
        CreatePropertyElements(#propertyList - #properties.elements)
    end

    for i=1, #properties.elements do
        local el = properties.elements[i]
        if i <= #propertyList then
            local investment = InvestmentDictionary.GetInvestment("property", propertyList[i])

            el.name.text = investment.name
            el.type.text = investment.sub_type
            el.condition.text = "100"
            el.age.text = investment.age
            el.shares.text = investment.shares
            el.value.text = ToCashString(investment.value)

            el.transform.gameObject.SetActive(true)
        else
            el.transform.gameObject.SetActive(false)
        end
    end
end

function CreatePropertyElements(amount)
    for i=1, amount do
        local object = Instantiate(properties.property_prefab, properties.content)
        object.gameObject.SetActive(true)
        properties.elements[#properties.elements + 1] = {
            transform = object.transform,
            name = object.transform.Find("Properties/Name").GetComponent("TextMeshProUGUI"),
            type = object.transform.Find("Properties/Type").GetComponent("TextMeshProUGUI"),
            condition = object.transform.Find("Properties/Condition").GetComponent("TextMeshProUGUI"),
            age = object.transform.Find("Properties/Age").GetComponent("TextMeshProUGUI"),
            shares = object.transform.Find("Properties/Shares").GetComponent("TextMeshProUGUI"),
            value = object.transform.Find("Properties/Value").GetComponent("TextMeshProUGUI")
        }
    end
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
