function OnReady()
end

-- Called when a company hires an employee with this job
function OnHired(company, employee)
    AddEmployeeLimit(company, employee, true);
end

-- Called when a company fires an employee with this job
function OnFired(company, employee)
    AddEmployeeLimit(company, employee, false);
end

function AddEmployeeLimit(company, employee, add)
    local effects = employee.Job.levels[employee.level+1].effects;
    if add then
        company.EmployeeLimit = company.EmployeeLimit + effects[1].amount;
    else
        company.EmployeeLimit = company.EmployeeLimit - effects[1].amount;
    end
end
