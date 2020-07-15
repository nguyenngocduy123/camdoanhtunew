select * from Customer where Code =  'YA849'; -- YA941 YA849


select * from camdoanhtu.history hs where Detail like N'%Kết thúc dây nợ ngày : 05/07/2020%';

select * from Customer where ID not in (select IDCus from Loan) and IsDeleted = 0 order by StartDate;

select hs.CustomerID, hs.Ngaydongtien, hs.Detail, cs.Code, cs.StartDate, hs.Price, cs.Price, cs.type
from camdoanhtu.history hs, Customer cs 
where cs.ID = hs.CustomerId and cs.Code in (select Code from Customer where ID not in (select IDCus from Loan) and IsDeleted = 0)
and hs.Detail like N'%Xóa%'
and hs.price != cs.Price
order by hs.CustomerID

select distinct cs.Code
from camdoanhtu.history hs, Customer cs 
where cs.ID = hs.CustomerId and cs.Code in (select Code from Customer where ID not in (select IDCus from Loan) and IsDeleted = 0)
and hs.Detail like N'%Xóa%'
and hs.price != cs.Price
order by hs.CustomerID

select *
from camdoanhtu.history hs, Customer cs 
where cs.ID = hs.CustomerId and cs.Code in (select Code from Customer where ID not in (select IDCus from Loan) and IsDeleted = 0)
and cs.Code = 'YA849';

select * from Loan where type = 1;

select * from camdoanhtu.history where CustomerId = 246;

select * from Loan where IDCus = 7298;