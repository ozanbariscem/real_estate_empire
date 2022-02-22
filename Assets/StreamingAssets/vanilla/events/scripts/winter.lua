tag = "" -- name of script, engine sets it, don't change
icon = "icons/fall.png"

-- CONDITIONS
local is_winter_time = false


local currently_active = false


function OnScriptLoaded()
    TimeManager.OnStartDateLoaded.add(HandleDateChange)
    TimeManager.OnMonthPass.add(HandleDateChange)
end

function OnEventFired()
    ConsoleRunCommand("log Hello you fired "..tag.." event.\n Currenty it gives you global homeless people KEKW")
    ModifierManager.AddGlobalModifier("property", "homeless", 0, 0, 3, 0)
end

function HandleDateChange(sender, date)
    is_winter_time = date.month == 12 or date.month == 1 or date.month == 2

    CheckForConditions()
end

function CheckForConditions()
    if is_winter_time then
        if not currently_active then
            EventManager.FireEvent(tag)
            currently_active = true
        end
    else
        currently_active = false
    end
end