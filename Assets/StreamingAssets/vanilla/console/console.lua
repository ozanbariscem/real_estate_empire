-- I exposed the Log function on DeveloperConsole.cs LoadScript function
function log_error(...)
    local string = params_to_string(...)
    Log("<color=#FF0000>"..string.."</color>")
end

function log(...)
    local string = params_to_string(...)
    Log(string)
end

function params_to_string(...)
    local params = {...}
    local string = ""
    for i=1, #params do 
        string = string..params[i].." "
    end
    return string
end

-- I exposed the UI class on DeveloperConsole.cs LoadScript function
function clear()
    UI.ClearLog()
end

function investment(...)
    local params = {...}

    local type = params[1]
    local id = tonumber(params[2])
    local investment = InvestmentDictionary.GetInvestment(type, id)
    if (investment == nil) then
        return
    end

    log(
        "\nbase_value: "..investment.base_value..
        "\nvalue: "..investment.value)
end
