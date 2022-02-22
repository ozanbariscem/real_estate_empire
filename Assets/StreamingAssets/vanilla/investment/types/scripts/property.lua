function OnReady()
    TimeManager.OnHourPass.add(HandleHourPass)
    TimeManager.OnDayPass.add(HandleDayPass)
    TimeManager.OnMonthPass.add(HandleMonthPass)
    TimeManager.OnYearPass.add(HandleYearPass)
end

function HandleHourPass(sender, date)
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

-- If you don't want to include any different behaviour for your custom 
-- investment type do NOT implement this function

-- This function is called every time a property value is calculated
-- id                      - id of the investment we are calculating for
-- base_value              - base_value of the investment we are calculating for
-- modifiers_effect        - how much does the modifiers effect value, [-1, 1]

-- Do not do any heavy calculations on this function since this might be called frequently
-- Trying to get many custom classes will also cause the CLR converter to go crazy and bottleneck
-- return value must be between 0 and 4,294,967,295
-- no single share of an investment should be worth more than this value

function CalculateValue(id, base_value, modifiers_effect)
    local value = base_value * modifiers_effect
    return value 
end
