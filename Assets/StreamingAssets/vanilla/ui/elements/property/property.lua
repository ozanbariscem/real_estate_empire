local transform -- Transform

local nameText  -- TextMeshProUGUI
local typeText  -- TextMeshProUGUI

local propertyImage -- RawImage
local ageText   -- TextMeshProUGUI
local shareText -- TextMeshProUGUI
local valueText -- TextMeshProUGUI

local piechartPrefab -- GameObject
local sharePrefab    -- GameObject

local shareParent     -- Transform
local piechartParent  -- Transform

local shareList = {}

onClicks =
{
    { "Summary/Close", "CloseMenu" }
}

-- When script is loaded from disk
function OnScriptLoaded()
    -- GameManager.GetTimeManager()
end

-- When script is assigned to gameobject
-- _transform : Transform
function OnScriptSet(_transform)
    transform = _transform

    nameText = transform.Find("Summary/PropertyNameText").GetComponent("TextMeshProUGUI")
    typeText = transform.Find("Summary/PropertyTypeText").GetComponent("TextMeshProUGUI")
    ageText = transform.Find("Properties/ConstructionText").GetComponent("TextMeshProUGUI")
    shareText = transform.Find("Properties/ShareText").GetComponent("TextMeshProUGUI")
    valueText = transform.Find("Properties/ValueText").GetComponent("TextMeshProUGUI")
    
    propertyImage = transform.Find("Properties/Image").GetComponent("RawImage")
    piechartPrefab = transform.Find("Owners/PiechartImage").gameObject
    sharePrefab = transform.Find("Owners/Share").gameObject

    shareParent = transform.Find("Owners/OwnerList/Scroll View/Viewport/Content")
    piechartParent = transform.Find("Owners/Piechart")

    CreateChartElements(56)

    SetHandlers()
    CloseMenu()
end

function OnClickEventsSet()
end

function SetHandlers()
    GameManager.GetMapManager().OnInvesmentClicked.add(HandleInvesmentClicked)
end

function CreateChartElements(count)
    for i=1, count do
        CreateChartElement()
    end

    -- reversing because shares with the least percentage should be on top
    for i=1, #shareList do
        shareList[i]["image"].transform.SetSiblingIndex(#shareList-i)
    end
end

function CreateChartElement()
    local piechart_object = Instantiate(piechartPrefab, piechartParent)
    local share_object = Instantiate(sharePrefab, shareParent)

    local share = CreateShare(piechart_object.transform, share_object.transform)

    shareList[#shareList + 1] = share
end

function CreateShare(image, share)
    local this = {
        transform = share,                                              -- Transform
        image     = image.GetComponent("Image"),                        -- Image
        dotImage  = share.Find("Image").GetComponent("Image"),          -- Image
        text      = share.Find("Text").GetComponent("TextMeshProUGUI"), -- TextMeshProUGUI 
    }
    return this
end

function HideShares()
    for i=1, #shareList do
        shareList[i]["transform"].gameObject.SetActive(false)
    end
end

-- sender : Object !IGNORE!
-- _invesment : Map.Invesment {Tag:string, Id:int}
function HandleInvesmentClicked(sender, _invesment)
    local invesment = InvesmentDictionary.GetInvesment(_invesment.Tag, _invesment.Id)
    local ownerships = OwnershipList.GetOwnershipsOfInvesment(_invesment.Tag, _invesment.Id)

    if ownerships == nil then return end

    UpdateTexture(invesment)
    UpdateTexts(invesment)
    UpdatePieChart(invesment, ownerships)

    transform.gameObject.SetActive(true)
end

function UpdateTexture(invesment)
    propertyImage.texture = invesment.texture
end

function UpdateTexts(invesment)
    local time = GameManager.GetTimeManager()
    local language = GameManager.GetLanguageManager()

    nameText.text = invesment.name
    typeText.text = language.Translate(invesment.sub_type:upper()):gsub("^%l", string.upper)

    ageText.text = language.Translate("CONSTRUCTION"):gsub("^%l", string.upper).." "..language.Translate("YEAR").." "..(time.date.year - invesment.age).." ("..invesment.age..")"
    
    valueText.text = language.Translate("PRICE"):gsub("^%l", string.upper).." "..invesment.baseValue
    if invesment.sub_type == "building" then
        shareText.text = invesment.shares.." "..language.Translate("APARTMENT").."s"
        valueText.text = language.Translate("APARTMENT"):gsub("^%l", string.upper).." "..(valueText.text:lower())
    end

    shareText.gameObject.SetActive(invesment.sub_type == "building")
    ageText.gameObject.SetActive(invesment.sub_type == "building" or invesment.sub_type == "house")
end

function UpdatePieChart(invesment, ownerships)
    if #ownerships > #shareList then
        local count = #ownerships - #shareList
        CreateChartElements(count)
    end

    local totalPercentage = 0
    for i=1, #ownerships do
        local ownershipAmount = ownerships[i].shares / invesment.shares
        local investor = InvestorList.GetInvestor(ownerships[i].investor_id)

        totalPercentage = totalPercentage + ownershipAmount

        shareList[i]["image"].fillAmount = totalPercentage
        shareList[i]["image"].gameObject.SetActive(true)
        
        shareList[i]["image"].color = investor.color
        shareList[i]["dotImage"].color = investor.color

        shareList[i]["text"].text = ""..(ownershipAmount*100).."% - "..investor.name
        shareList[i]["transform"].gameObject.SetActive(true)
    end
end

function CloseMenu()
    transform.gameObject.SetActive(false)
    HideShares()
end
