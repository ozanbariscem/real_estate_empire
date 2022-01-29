-- IMPORTANT NOTICE: Log function can't be seen by the modder since it's using Debug.Log of Unity, change that to custom console logging

-- Has access to ChangeInterval, Log functions and 
-- Calendar.Month, Calendar.Calendar, Date, Interval, Intervals classes with their functions
-- But remember, with great power comes what? Finish it.

-- These events are called after their c# counterpart

-- Also a little self note for the future this whole class could be handled with this Lua script
-- Since it's not a heavy class and doesn't require the speed of c#

local date

-- First ever streaming asset loaded by the c# Time.Manager classes
-- Pretty much nothing is loaded at this point
function OnScriptLoaded()
end

function OnCalendarLoaded(calendar)
end

function OnIntervalsLoaded(intervals)
end

-- If I don't change it in the future this is the last loaded asset
-- So at this point whole class is ready
function OnStartDateLoaded(_date)
    date = _date
end


function OnIntervalChanged(to_index)
end

function OnIntervalLooped()
end

-- Given as an example for modder
function OnDayPass(new_day)
    -- Log(date.ToNumberString())
end

function OnMonthPass(new_month)
end

function OnYearPass(new_year)
end
