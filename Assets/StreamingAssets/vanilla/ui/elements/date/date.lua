-- GameManager GameManager
local transform -- Transform
local text -- TextMeshProUGUI

local speedIndicators = {} -- Transform[]
local speedParent -- Transform
local pauseText -- TextMeshProUGUI

local pauseImage -- Transform
local playImage  -- Transform

local timeManager

onClicks = {
    { "TextButton", "ToggleTime" },
    { "Buttons/Content/Increase", "IncreaseSpeed" },
    { "Buttons/Content/Decrease", "DecreaseSpeed" }
}
   

-- When script is loaded from disk
function OnScriptLoaded()
    timeManager = GameManager.GetTimeManager()
end

-- When script is assigned to gameobject
-- _transform : Transform
function OnScriptSet(_transform)
    transform = _transform

    text = transform.Find("Text").GetComponent("TextMeshProUGUI")
    pauseText = transform.Find("PauseText").GetComponent("TextMeshProUGUI")

    GetSpeedIndicators()
    GetTimeStateImages()

    SetHandlers()
end

function OnClickEventsSet()
end

function GetSpeedIndicators()
    speedParent = transform.Find("Speed")
    for i=1, speedParent.childCount do
        speedIndicators[i] = speedParent.GetChild(i-1)
    end
end

function GetTimeStateImages()
    pauseImage = transform.Find("PauseState")
    playImage = transform.Find("PlayState")
end

function SetHandlers()
    timeManager.onHourPass.add(HandleHourPass)
    timeManager.OnIntervalChanged.add(HandleIntervalChange)
    timeManager.OnPaused.add(HandlePause)
    timeManager.OnResumed.add(HandleResume)
end

-- sender : Not exposed don't use
-- date : Date
function HandleHourPass(sender, date)
    text.text = ""..date.ToNumberString()
end

-- sender : Not exposed don't use
-- intervals : Intervals
function HandleIntervalChange(sender, intervals)
    for i=1, #speedIndicators do
        speedIndicators[i].gameObject.SetActive(false)
        if i <= intervals.SelectedInterval + 1 then
            speedIndicators[i].gameObject.SetActive(true)
        end
    end
end

function HandlePause()
    HandleTimeStateChanged(true)
end

function HandleResume()
    HandleTimeStateChanged(false)
end

function HandleTimeStateChanged(paused)
    speedParent.gameObject.SetActive(not paused);
    pauseText.gameObject.SetActive(paused);
    pauseImage.gameObject.SetActive(paused);
    playImage.gameObject.SetActive(not paused);
end

function ToggleTime()
    if timeManager.IsPaused then
        timeManager.Play()
    else
        timeManager.Pause()
    end
end

function IncreaseSpeed()
    timeManager.ChangeInterval(timeManager.Intervals.SelectedInterval + 1)
end

function DecreaseSpeed()
    timeManager.ChangeInterval(timeManager.Intervals.SelectedInterval - 1)
end