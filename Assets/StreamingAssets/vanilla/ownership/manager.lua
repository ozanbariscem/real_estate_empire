-- ConsoleRunCommand(command:string)

function OnScriptLoaded()
end

-- ownerships : List<Ownership>
function OnOwnershipsLoaded(ownerships)
    for i=1, #ownerships do
        ConsoleRunCommand("log "..ownerships[i].invesment_type)
    end
end
