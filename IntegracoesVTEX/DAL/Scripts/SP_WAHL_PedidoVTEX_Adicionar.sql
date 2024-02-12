alter procedure SP_WAHL_PedidoVTEX_Adicionar
(
	@NUMERO_PEDIDO varchar(20),
	@EVENTO varchar(100)
)
as 
begin

declare @total int = (select count(*) from [@WAHL_VTEX_PEDIDOS] where U_NUM_VTEX = @NUMERO_PEDIDO);

	if (@total = 0)
	begin

		insert into [@WAHL_VTEX_PEDIDOS]
		(Code, Name, U_NUM_VTEX, U_DATA, U_STATUS, U_EVENTO)
		values 
		(
			(select count(*) + 1 from [@WAHL_VTEX_PEDIDOS]),
			(select count(*) + 1 from [@WAHL_VTEX_PEDIDOS]),
			@NUMERO_PEDIDO,
			GETDATE(),
			0,
			@EVENTO
		);

	end

end