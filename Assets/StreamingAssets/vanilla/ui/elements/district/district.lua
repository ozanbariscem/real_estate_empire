local transform
local element

local basic;
local detail;

local property_element_amount = 30

onClicks = {
}

onHovers = {
}

function OnScriptLoaded()
end

function OnScriptSet(_transform)
    transform = _transform
    element = transform.GetComponent("Element")

    GetBasicElements(transform)
    GetDetailElements(transform)
    SetHandlers()
    -- CreatePropertyElements(property_element_amount)

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

    basic.district_text.text = district.Data.name
    detail.top.district_text.text = district.Data.name

    detail.top.avgPrice_text.text = "-"
    detail.top.buildings_text.text = district.Data.size
    detail.top.population_text.text = district.population

    -- UpdatePropertyElements(district.properties)
    UIManager.OpenMenu("GameMenu/district/")
end

function SetHandlers()
    MapDistrict.OnClicked.add(HandleDistrictClicked)
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

function GetBasicElements(parent)
    basic = {
        district_text = parent.Find("Summary/Name").GetComponent("TextMeshProUGUI")
    }
end

function GetDetailElements(parent)
    detail = {
        top = {
            district_text = parent.Find("Detail/Top/Top/Name").GetComponent("TextMeshProUGUI"),
            avgPrice_text = parent.Find("Detail/Top/Bot/AvgPrice/Text").GetComponent("TextMeshProUGUI"),
            buildings_text = parent.Find("Detail/Top/Bot/Building/Text").GetComponent("TextMeshProUGUI"),
            population_text = parent.Find("Detail/Top/Bot/Population/Text").GetComponent("TextMeshProUGUI")
        },
        bot = {
            buildings = {
                text = parent.Find("Detail/Bot/Right/Buildings/Text").GetComponent("TextMeshProUGUI"),
                value = parent.Find("Detail/Bot/Right/Buildings/Value").GetComponent("TextMeshProUGUI")
            },
            properties = {
                text = parent.Find("Detail/Bot/Right/Properties/Text").GetComponent("TextMeshProUGUI"),
                value = parent.Find("Detail/Bot/Right/Properties/Value").GetComponent("TextMeshProUGUI")
            },
            total_value = {
                text = parent.Find("Detail/Bot/Right/TotalValue/Text").GetComponent("TextMeshProUGUI"),
                value = parent.Find("Detail/Bot/Right/TotalValue/Value").GetComponent("TextMeshProUGUI")
            },
            modifiers = {
                text = parent.Find("Detail/Bot/Right/Modifiers/Text").GetComponent("TextMeshProUGUI"),
                content = parent.Find("Detail/Bot/Right/Modifiers/List/Viewport/Content"),
                prefab = parent.Find("Detail/Bot/Right/Modifiers/List/Viewport/Content/Image")
            }
        },
        left = {
            pie = {
                chart = parent.Find("Detail/Bot/Left/PieChart"),
                prefab = parent.Find("Detail/Bot/Left/PieChart/Image"),
            },
            company = {
                content = parent.Find("Detail/Bot/Left/List/Viewport/Content"),
                prefab = parent.Find("Detail/Bot/Left/List/Viewport/Content/Company")
            }
        }
    }
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
