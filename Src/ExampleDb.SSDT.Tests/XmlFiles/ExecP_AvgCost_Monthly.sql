declare @outputid INT;

exec [P_AvgCost_Monthly] @touid, @costid, @outputid OUTPUT;

select [DateTime],[Value] from timeserie where outputid = @outputid