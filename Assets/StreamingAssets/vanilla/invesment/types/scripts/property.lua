-- This scripts controls the behaviour of property invesments
-- Don't remove this we don't null check (TODO: null check)
function OnReady()
end

-- I could seamlessly(~0.000001 runtime) loop through and Log 3K invesments on my machine
-- But since a day pass might be frequent, I don't advise to
-- do heavy calculations in this function
--function HandleHourPass(hour, invesments)
    -- It's better to remove empty function bodies entirely for performance reasons
--end

--function HandleDayPass(day, invesments)
    -- It's better to remove empty function bodies entirely for performance reasons
--end

--function HandleMonthPass(month, invesment)
    -- It's better to remove empty function bodies entirely for performance reasons
--end

function HandleYearPass(year, invesments)
    for i = 1, #invesments do
        if invesments[i].Data.Is("ageable") then
            invesments[i].age = invesments[i].age + 1
        end
    end
end
