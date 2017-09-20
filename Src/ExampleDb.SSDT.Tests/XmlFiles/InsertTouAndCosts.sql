insert into [output] default values;
select @touid = @@identity; 

insert into [output] default values;
select @costid = @@identity; 

-- insert costs
insert into [timeserie] (outputid, [datetime], [value])	values (@costid, @dt, 10)
insert into [timeserie] (outputid, [datetime], [value])	values (@costid, @dt+1, 20)
insert into [timeserie] (outputid, [datetime], [value])	values (@costid, @dt+2, 60)
insert into [timeserie] (outputid, [datetime], [value])	values (@costid, @dt+3, 100)

-- insert tous
insert into [TimeSerie] (outputid, [datetime], [value]) values(@touid, @dt, 1)
insert into [TimeSerie] (outputid, [datetime], [value]) values(@touid, @dt+1, 1)
insert into [TimeSerie] (outputid, [datetime], [value]) values(@touid, @dt+2, 1)
insert into [TimeSerie] (outputid, [datetime], [value]) values(@touid, @dt+3, 0)