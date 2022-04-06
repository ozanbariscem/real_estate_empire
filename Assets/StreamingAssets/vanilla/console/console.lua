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
    
    if (params[1] == "apartments") then
        -- params[2] => page of investment list 
        -- (We use pagination we can't list 100s of investments because memory allocation goes crazy)
        Debug.ListApartments(tonumber(params[2]))
        return
    end

    if (params[1] == "buildings") then
        -- params[2] => page of investment list 
        -- (We use pagination we can't list 100s of investments because memory allocation goes crazy)
        Debug.ListBuildings(tonumber(params[2]))
        return
    end

    if (params[1] == "jobs") then
        -- params[2] => page of job
        Debug.ListJobs(tonumber(params[2]))
        return
    end

    if (params[1] == "employees") then
        -- params[2] => tag of company
        -- params[3] => page of employees
        Debug.ListEmployees(params[2], tonumber(params[3]))
        return
    end

    if (params[1] == "employment") then
        -- params[2] => page of employment
        Debug.ListEmployments(tonumber(params[2]))
        return
    end

    if (params[1] == "names") then
        -- params[2] => page of names
        Debug.ListNames(tonumber(params[2]))
        return
    end

    if (params[1] == "surnames") then
        -- params[2] => page of names
        Debug.ListSurnames(tonumber(params[2]))
        return
    end
end

function hire(...)
    local params = {...}

    local company = params[1] -- company tag
    local employee = tonumber(params[2]) -- employee id

    EmploymentManager.Hire(company, employee)
end

function fire(...)
    local params = {...}

    local company = params[1] -- company tag
    local employee = tonumber(params[2]) -- employee id

    EmploymentManager.Fire(company, employee)
end
