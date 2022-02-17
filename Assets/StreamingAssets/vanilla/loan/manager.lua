function OnScriptLoaded()
    LoanManager.OnTypesLoaded.add(HandleTypesLoaded)
    LoanManager.OnLoansLoaded.add(HandleLoansLoaded)
end

function OnRulesLoaded()
end

function OnContentLoaded()
end

-- types:Dictinary<int, Type>
function HandleTypesLoaded(sender, types)
    -- for i=0, #types do
    --     ConsoleRunCommand("log "..types[i].name)
    -- end
end

-- loans:Dictinary<int, Loan>
function HandleLoansLoaded(sender, loans)
    -- for i=0, #loans do
    --     ConsoleRunCommand("log "..loans[i].Type.type)
    --     ConsoleRunCommand("log "..loans[i].amountLeft)
    -- end
end
