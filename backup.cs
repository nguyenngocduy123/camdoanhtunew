 #region test 

                List<int> list = new List<int>();
                string line;

                System.IO.StreamReader file = new System.IO.StreamReader(@"C:\text.txt");

                while ((line = file.ReadLine()) != null)
                {
                    list.Add(Int32.Parse(line));
                }

                foreach (var item in list)
                {
                    var hs = ctx.histories.Where(p => p.ID == item).ToList();
                    //var cs = ctx.Customers.Where(p => p.Code == item).FirstOrDefault();
                    if (hs != null && hs.Count > 0)
                    {
                        //var loans = ctx.Loans.Where(p => p.IDCus == cs.ID).ToList();
                        //if (loans != null && loans.Count > 0) continue;
                        //int daypaid = cs.DayPaids.Value;
                        //float day =
                        //    float.Parse(cs.Loan.ToString()) / float.Parse(cs.Price.ToString());

                        for (int i = 0; i < hs.Count; i++)
                        {
                            Loan temp = new Loan();
                            temp.Date = cs.StartDate.AddDays(i);
                            temp.IDCus = hs[i].CustomerId;
                            temp.Status = 1;
                            temp.Type = true;
                            temp.money = 1;
                            ctx.Loans.Add(temp);
                        }
                        ctx.SaveChanges();
                    }
                }

                file.Close();

                return View();

                #endregion