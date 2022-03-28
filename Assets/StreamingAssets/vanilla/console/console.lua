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

function debug(...)
    local params = {...}

    if (params[1] == "true" or params[1] == "false") then
        local bool = params[1]
        Debug.Set(bool == "true")
        return
    end

    if (not Debug.State.IsActive) then
        log_error("Please activate debug mode 'debug true' before trying to debug.")
        return
    end
    
    if (params[1] == "investment") then
        -- params[2] => type of investment
        -- params[3] => page of investment list 
        -- (We use pagination we can't list 100s of investments because memory allocation goes crazy)
        Debug.ListInvestments(params[2], tonumber(params[3]))
    end
end
