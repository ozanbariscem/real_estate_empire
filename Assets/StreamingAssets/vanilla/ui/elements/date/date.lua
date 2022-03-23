-- GameManager GameManager
local transform -- Transform

local dateMenu = {
    dateText = nil,
    hourText = nil,
    pauseButton = nil,
    playButton = nil
}

local speedMenu = {
    speed = { 
        parent = nil,
        indicators = { nil, nil, nil, nil, nil }
    }
}

onClicks = {
    { "", "ToggleTime" },
    { "Date/Speeds/0", "SetSpeed0" },
    { "Date/Speeds/1", "SetSpeed1" },
    { "Date/Speeds/2", "SetSpeed2" },
    { "Date/Speeds/3", "SetSpeed3" },
    { "Date/Speeds/4", "SetSpeed4" },
}

function OnScriptLoaded()
end

function OnScriptSet(_transform)
    transform = _transform

    dateMenu = {
        dateText = transform.Find("Date/Date/Date").GetComponent("TextMeshProUGUI"),
        hourText = transform.Find("Date/Hour/Hour").GetComponent("TextMeshProUGUI"),
        pauseButton = transform.Find("Buttons/PauseButton"),  -- Not actually button
        playButton = transform.Find("Buttons/PlayButton")     -- Not actually button
    }

    speedMenu = {
        speed = { 
            parent = transform.Find("Date/Speeds/"),
            indicators = { 
                transform.Find("Date/Speeds/0/Image").GetComponent("Image"), 
                transform.Find("Date/Speeds/1/Image").GetComponent("Image"), 
                transform.Find("Date/Speeds/2/Image").GetComponent("Image"), 
                transform.Find("Date/Speeds/3/Image").GetComponent("Image"), 
                transform.Find("Date/Speeds/4/Image").GetComponent("Image") 
            }
        }
    }

   SetHandlers()
end

function SetHandlers()
    TimeManager.OnHourPass.add(HandleHourPass)
    TimeManager.OnIntervalChanged.add(HandleIntervalChange)
    TimeManager.OnPaused.add(HandlePause)
    TimeManager.OnResumed.add(HandleResume)
end

function HandleHourPass(sender, date)
    dateMenu.dateText.text = date.ToMenuStringDate()
    dateMenu.hourText.text = date.ToMenuStringHour()
end

function HandleIntervalChange(sender, intervals)
    for i=1, #speedMenu.speed.indicators do
        speedMenu.speed.indicators[i].color = Color(120/255, 120/255, 120/255, 1)
        if i <= intervals.SelectedInterval + 1 then
            speedMenu.speed.indicators[i].color = Color(1, 1, 1, 1)
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
    dateMenu.playButton.gameObject.SetActive(paused)
    dateMenu.pauseButton.gameObject.SetActive(not paused)
end

function ToggleTime()
    if TimeManager.IsPaused then
        TimeManager.Play()
    else
        TimeManager.Pause()
    end
end

function SetSpeed4()
    SetSpeed(4)
end

function SetSpeed3()
    SetSpeed(3)
end

function SetSpeed2()
    SetSpeed(2)
end

function SetSpeed1()
    SetSpeed(1)
end

function SetSpeed0()
    SetSpeed(0)
end

function SetSpeed(index)
    -- TimeManager.ChangeInterval(TimeManager.Intervals.SelectedInterval + 1)
    TimeManager.ChangeInterval(index)
end
