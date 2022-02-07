local transform

local backgroundImage
local progressText
local hintText

onClicks = {}

function OnScriptLoaded()
    LoadManager.OnProgressStart.add(HandleProgressStart)
    LoadManager.OnLoadProgressed.add(HandleLoadProgressed)
    LoadManager.OnProgressFinish.add(HandleProgressFinish)
end

function OnScriptSet(_transform)
    transform = _transform

    backgroundImage = transform.Find("Background").GetComponent("RawImage")
    progressText = transform.Find("Progress/Text").GetComponent("TextMeshProUGUI")
    hintText = transform.Find("Tips/Text").GetComponent("TextMeshProUGUI")
end

function OnClickEventsSet()
end

function HandleLoadProgressed(sender, progress)
    progressText.text = progress.message
end

function HandleProgressStart(sender)
    UIManager.loadMenu.gameObject.SetActive(true)
end

function HandleProgressFinish(sender)
    UIManager.loadMenu.gameObject.SetActive(false)
end
