CREATE procedure [SP_WAHL_PedidoVTEX_Status]
(
	@NUMERO_PEDIDO varchar(20),
	@status int
)
as 
begin

	update [@WAHL_VTEX_PEDIDOS]
	set U_STATUS = @status
	where U_NUM_VTEX = @NUMERO_PEDIDO;

end
GO


