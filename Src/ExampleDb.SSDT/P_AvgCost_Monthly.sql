CREATE PROCEDURE [dbo].[P_AvgCost_Monthly]
	@tou_outputid int,
	@cost_outputid int,
	@outputid int output
AS

	insert into dbo.[Output] default values;
	select @outputid = @@identity

	insert into [TimeSerie] (OutputId, [DateTime], [Value])
		select
		  @outputid,
		  DATEADD(month, DATEDIFF(month, 0, tou.[DateTime]), 0),
		  avg(COST.Value)
		from
		  dbo.TimeSerie as TOU
		  INNER JOIN TimeSerie as COST 
			ON COST.OutputId = @cost_outputid
			and TOU.[DateTime] = COST.[DateTime]
			and TOU.[Value] = 1
		where
		  TOU.OutputId = @tou_outputid
		GROUP by
		  DATEADD(month, DATEDIFF(month, 0, TOU.[DateTime]), 0)
	
