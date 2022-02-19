local properties

function OnReady()
    TimeManager.OnHourPass.add(HandleHourPass)
    TimeManager.OnDayPass.add(HandleDayPass)
    TimeManager.OnMonthPass.add(HandleMonthPass)
    TimeManager.OnYearPass.add(HandleYearPass)

    InvestmentManager.OnInvestmentsLoaded.add(HandleInvestmentsLoaded)
end

function HandleInvestmentsLoaded(sender, investments)
    properties = investments["property"]
end

function HandleHourPass(sender, date)
    -- properties at base are investments and investments class doesn't know which district they belong to
    -- investments might have modifiers and so might the districts 
    for i = 1, #properties do
        if properties[i].Data.Is("ageable") then
            properties[i].age = properties[i].age + 1
        end
    end
end

function HandleDayPass(sender, date)
end

function HandleMonthPass(sender, date)
end

function HandleYearPass(sender, date)
    for i = 1, #properties do
        if properties[i].Data.Is("ageable") then
            properties[i].age = properties[i].age + 1
        end
    end
end
