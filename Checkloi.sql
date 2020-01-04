use camcdb3d_camdothanhluan
use camdoanhtu;

select * from Customer cs
where cs.DayPaids != (
select count(*) from Loan l,Customer c 
where l.Status = 1 and l.Type = 0 and c.ID = l.IDCus and c.Code = cs.Code) 
and cs.type !=12 and cs.type !=13 and cs.type !=14 and cs.type !=15 and cs.type !=16 and cs.type !=17 ;

select * from Customer cs
where cs.AmountPaid != (
select (count(*)*c.Price) as amountpaid from Loan l,Customer c 
where l.Status = 1 and l.Type = 0 and c.ID = l.IDCus and c.Code = cs.Code
and cs.type !=12 and cs.type !=13 and cs.type !=14 and cs.type !=15 and cs.type !=16 and cs.type !=17 
group by c.Price);



select * from Message;

select Id,* from Customer where Code = 'MA448';

select * from Loan where IDCus = 5781;

select * from History where customerid = 5781;

