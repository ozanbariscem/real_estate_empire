-- This scripts controls the behaviour of property invesments

function OnReady()
end

-- I could seamlessly(~0.000001 runtime) loop through and Log 3K invesments on my machine
-- But since a day pass might be frequent, I don't advise to
-- do heavy calculations in this function
function HandleDayPass(day, invesments)
    Log("Test code, REMOVE!")
    for i = 1, #invesments do
        if invesments[i].Data.Is("ageable") then
            invesments[i].age = invesments[i].age + 1
        end
    end
end

function HandleMonthPass(month, invesment)
end

function HandleYearPass(year, invesment)
    for i = 1, #invesments do
        if invesments[i].Data.Is("ageable") then
            invesments[i].age = invesments[i].age + 1
        end
    end
end
