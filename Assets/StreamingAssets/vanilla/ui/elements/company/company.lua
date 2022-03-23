local transform
local element

local asset_elements_amount_at_start = 30

local menu = {
    topbar = {
        header = nil
    },
    information = {
        finance = {
            header = nil,
            funds = {
                key = nil,
                value = nil
            },
            assets = {
                key = nil,
                value = nil
            },
            debt = {
                key = nil,
                value = nil
            },
            networth = {
                key = nil,
                value = nil
            }
        },
        assets = {
            header = nil,
            property = {
                key = nil,
                value = nil
            }
        }
    },
    submenus = {
        assets = {
            prefab = nil, 
            content = nil,
            elements = { }
        }
    }
}

onClicks = {
    { "", "HandleMenuClicked" }
}

function OnScriptLoaded()
end

function OnScriptSet(_transform)
    transform = _transform
    transform.gameObject.SetActive(false)
    element = transform.GetComponent("Element")

    menu = {
        topbar = {
            header = transform.Find("Topbar/Header").GetComponent("TextMeshProUGUI")
        },
        information = {
            finance = {
                header = transform.Find("Information/Content/Finance/Header").GetComponent("TextMeshProUGUI"),
                funds = {
                    key = transform.Find("Information/Content/Finance/Funds/Key").GetComponent("TextMeshProUGUI"),
                    value = transform.Find("Information/Content/Finance/Funds/Value").GetComponent("TextMeshProUGUI")
                },
                assets = {
                    key = transform.Find("Information/Content/Finance/Assets/Key").GetComponent("TextMeshProUGUI"),
                    value = transform.Find("Information/Content/Finance/Assets/Value").GetComponent("TextMeshProUGUI"),
                },
                debt = {
                    key = transform.Find("Information/Content/Finance/Debt/Key").GetComponent("TextMeshProUGUI"),
                    value = transform.Find("Information/Content/Finance/Debt/Value").GetComponent("TextMeshProUGUI"),
                },
                networth = {
                    key = transform.Find("Information/Content/Finance/Networth/Key").GetComponent("TextMeshProUGUI"),
                    value = transform.Find("Information/Content/Finance/Networth/Value").GetComponent("TextMeshProUGUI"),
                }
            },
            assets = {
                header = transform.Find("Information/Content/Assets/Header").GetComponent("TextMeshProUGUI"),
                property = {
                    key = transform.Find("Information/Content/Assets/Property/Key").GetComponent("TextMeshProUGUI"),
                    value = transform.Find("Information/Content/Assets/Property/Value").GetComponent("TextMeshProUGUI"),
                }
            }
        },
        submenus = {
            assets = {
                prefab = transform.Find("SubMenus/Assets/Scroll View/Viewport/Content/AssetPrefab").gameObject, 
                content = transform.Find("SubMenus/Assets/Scroll View/Viewport/Content"),
                elements = { }
            }
        }
    }

    SetAssetsSubMenu()
end

function HandleMenuClicked()
    UIManager.BringToFront(element)
end

function OnActivate(param)
    UpdateMenu(param)
end

function OnDeactivate()
end

function UpdateMenu(company)
    menu.topbar.header.text = company.name

    menu.information.finance.funds.value.text = ToCashString(company.cash)
    menu.information.finance.assets.value.text = ToCashString(company.assetsWorth)
    menu.information.finance.debt.value.text = ToCashString(company.debt)
    menu.information.finance.networth.value.text = ToCashString(company.Networth)

    menu.information.assets.property.value.text = OwnershipDictionary.GetTotalAssetsOfCompany(company.tag, "property").." shares"

    local assets = OwnershipDictionary.GetAssetsOfCompany(company.tag)
    UpdateAssetElements(assets)
end

function SetAssetsSubMenu()
    menu.submenus.assets.prefab.SetActive(false)
    CreateAssetElements(asset_elements_amount_at_start)
end

function CreateAssetElements(amount)
    local prefab = menu.submenus.assets.prefab
    for i=1, amount do
        local object = Instantiate(prefab, menu.submenus.assets.content)
        menu.submenus.assets.elements[#menu.submenus.assets.elements+1] = {
            transform = object.transform,
            type = object.transform.Find("Type").GetComponent("TextMeshProUGUI"),
            name = object.transform.Find("Name").GetComponent("TextMeshProUGUI"),
            shares = object.transform.Find("Shares").GetComponent("TextMeshProUGUI"),
            value = object.transform.Find("Value").GetComponent("TextMeshProUGUI"),
            button = object.transform.Find("Button")
        }
    end
end

function UpdateAssetElements(assets)
    if #menu.submenus.assets.elements < #assets then
        CreateAssetElements(#assets - #menu.submenus.assets.elements)
    end

    for i=1, #menu.submenus.assets.elements do
        local el = menu.submenus.assets.elements[i]
        if i <= #assets then
            local investment = InvestmentDictionary.GetInvestment(assets[i].investment_type, assets[i].investment_id)

            el.type.text = assets[i].investment_type
            el.name.text = investment.name
            el.shares.text = assets[i].shares
            el.value.text = ToCashString(assets[i].shares*investment.value)

            el.transform.gameObject.SetActive(true)
        else
            el.transform.gameObject.SetActive(false)
        end
    end
end