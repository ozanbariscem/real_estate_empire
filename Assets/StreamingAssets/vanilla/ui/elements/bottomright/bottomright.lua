local transform;

onClicks = {
    { "Date/Date/Button", "ToggleTime" },
    { "Date/State/Play", "ToggleTime" },
    { "Date/State/Pause", "ToggleTime" },
    { "Date/Speeds/0", "SetSpeed0" },
    { "Date/Speeds/1", "SetSpeed1" },
    { "Date/Speeds/2", "SetSpeed2" },
    { "Date/Speeds/3", "SetSpeed3" },
    { "Date/Speeds/4", "SetSpeed4" },
}

onHovers = {
    { "Date/Date", "HandleDateHovered" },
    { "Date/State", "HandleStateHovered" },
    { "Date/Speeds", "HandleSpeedsHovered" },
}

local green;
local red;
local grey;

function OnScriptLoaded()
end

function OnScriptSet(_transform)
    transform = _transform

    dateMenu = {
        dateText = transform.Find("Date/Date/Text").GetComponent("TextMeshProUGUI"),
        pauseButton = transform.Find("Date/State/Pause"),
        playButton = transform.Find("Date/State/Play")
    }

    speedMenu = {
        indicators = { 
            transform.Find("Date/Speeds/0").GetComponent("Image"), 
            transform.Find("Date/Speeds/1").GetComponent("Image"), 
            transform.Find("Date/Speeds/2").GetComponent("Image"), 
            transform.Find("Date/Speeds/3").GetComponent("Image"), 
            transform.Find("Date/Speeds/4").GetComponent("Image") 
        }
    }

    grey = Color(180/255, 180/255, 180/255, 222/255);
    green = Color(56/255, 161/255, 93/255, 1);
    red = Color(183/255, 84/255, 84/255, 1);

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
end

function ToggleTime()
    if TimeManager.IsPaused then
        TimeManager.Play()
    else
        TimeManager.Pause()
    end
end

function HandleIntervalChange(sender, intervals)
    for i=1, #speedMenu.indicators do
        speedMenu.indicators[i].color = grey
        if i <= intervals.SelectedInterval + 1 then
            if TimeManager.IsPaused then
                speedMenu.indicators[i].color = red
            else
                speedMenu.indicators[i].color = green
            end
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

    HandleIntervalChange(null, TimeManager.Intervals)

    if paused then
        dateMenu.dateText.color = red
    else
        dateMenu.dateText.color = grey
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
    TimeManager.ChangeInterval(index)
end

function HandleDateHovered()
    return {
        header = LanguageManager.Translate("DATE_HOVER_TITLE"),
        description = "\n"..TimeManager.Date.ToMenuStringHour().." / "..TimeManager.Date.ToMenuStringDate()
    }
end

function HandleStateHovered()
    local desc = LanguageManager.Translate("PLAYING")
    if TimeManager.IsPaused then
        desc = LanguageManager.Translate("PAUSED")
    end

    return {
        header = LanguageManager.Translate("STATE_HOVER_TITLE"),
        description = "\n"..desc
    }
end

function HandleSpeedsHovered()
    local speed = TimeManager.Intervals.SelectedInterval + 1
    return {
        header = LanguageManager.Translate("SPEEDS_HOVER_TITLE"),
        description = "\n"..LanguageManager.Translate("SPEEDS_HOVER_DESCRIPTION")..speed
    }
end