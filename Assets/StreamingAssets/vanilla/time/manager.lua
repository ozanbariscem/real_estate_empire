function OnScriptLoaded()
    TimeManager.OnCalendarLoaded.add(HandleCalendarLoaded)
    TimeManager.OnIntervalsLoaded.add(HandleIntervalsLoaded)
    TimeManager.OnStartDateLoaded.add(HandleStartDateLoaded)
    
    TimeManager.OnPaused.add(HandlePaused)
    TimeManager.OnResumed.add(HandleResumed)
    
    TimeManager.OnIntervalChanged.add(HandleIntervalChanged)
    
    TimeManager.OnIntervalLooped.add(HandleIntervalLooped)
    TimeManager.OnHourPass.add(HandleHourPass)
    TimeManager.OnDayPass.add(HandleDayPass)
    TimeManager.OnMonthPass.add(HandleMonthPass)
    TimeManager.OnYearPass.add(HandleYearPass)
end

function OnRulesLoaded()
end

function OnContentLoaded()
end

function HandleCalendarLoaded(sender, calendar)
end

function HandleIntervalsLoaded(sender, intervals)
end

function HandleStartDateLoaded(sender, date)
end

function HandlePaused()
end

function HandleResumed()
end

function HandleIntervalChanged(sender, intervals)
end

function HandleIntervalLooped()
end

function HandleHourPass(sender, date)
end

function HandleDayPass(sender, date)
end

function HandleMonthPass(sender, date)
end

function HandleYearPass(sender, date)
end
