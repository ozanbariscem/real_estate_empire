-- GameManager GameManager
local transform -- Transform

local dateText
local hourText

local speedIndicators = {} -- Transform[]
local speedParent -- Transform
local pauseText -- TextMeshProUGUI

onClicks = {
    { "PauseButton", "ToggleTime" },
    { "GameSpeedButtons/Content/Increase", "IncreaseSpeed" },
    { "GameSpeedButtons/Content/Decrease", "DecreaseSpeed" }
}

-- When script is loaded from disk
function OnScriptLoaded()
end

-- When script is assigned to gameobject
-- _transform : Transform
function OnScriptSet(_transform)
    transform = _transform

    dateText = transform.Find("DateMenu/Date/Date").GetComponent("TextMeshProUGUI")
    hourText = transform.Find("DateMenu/Hour/Date").GetComponent("TextMeshProUGUI")

    pauseText = transform.Find("GameSpeedMenu/PauseText").GetComponent("TextMeshProUGUI")
    pauseText.text = LanguageManager.Translate("PAUSED"):upper()

    GetSpeedIndicators()
    SetHandlers()
end

function OnClickEventsSet()
end

function GetSpeedIndicators()
    speedParent = transform.Find("GameSpeedMenu/Speed")
    for i=1, speedParent.childCount do
        speedIndicators[i] = speedParent.GetChild(i-1)
    end
end

function SetHandlers()
    TimeManager.onHourPass.add(HandleHourPass)
    TimeManager.OnIntervalChanged.add(HandleIntervalChange)
    TimeManager.OnPaused.add(HandlePause)
    TimeManager.OnResumed.add(HandleResume)
end

-- sender : Not exposed don't use
-- date : Date
function HandleHourPass(sender, date)
    dateText.text = date.day.."."..date.month.."."..date.year
    hourText.text = date.hour..":00"
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
    speedParent.gameObject.SetActive(not paused)
    pauseText.gameObject.SetActive(paused)
end

function ToggleTime()
    if TimeManager.IsPaused then
        TimeManager.Play()
    else
        TimeManager.Pause()
    end
end

function IncreaseSpeed()
    TimeManager.ChangeInterval(TimeManager.Intervals.SelectedInterval + 1)
end

function DecreaseSpeed()
    TimeManager.ChangeInterval(TimeManager.Intervals.SelectedInterval - 1)
end