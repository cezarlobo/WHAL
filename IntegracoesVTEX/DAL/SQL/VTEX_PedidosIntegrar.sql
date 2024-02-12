select 
	U_NUM_VTEX,
	U_EVENTO
from 
	[@WAHL_VTEX_PEDIDOS] 
where 
	U_STATUS in (0,2) 
order by
	U_DATA 