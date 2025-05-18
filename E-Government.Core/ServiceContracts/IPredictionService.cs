using E_Government.Core.Domain.Entities.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Core.ServiceContracts
{
  public  interface IPredictionService
    {
        BillPrediction Predict(BillData input);

    }
}
