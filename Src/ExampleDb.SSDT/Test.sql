CREATE PROCEDURE [dbo].[Test]
	@param1 int OUTPUT
AS
	SELECT @param1 = 12
