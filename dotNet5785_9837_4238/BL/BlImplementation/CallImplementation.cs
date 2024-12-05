namespace BlImplementation;
using BlApi;
using BO;
using Helpers;
using Microsoft.VisualBasic;
using System.Collections.Generic;
using System.Text;

internal class CallImplementation : ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public void ChoiceOfCallToCare(int volId, int callId)
    {
        try
        {
            // שליפת הקריאה והמתנדב משכבת הנתונים
            DO.Call? call = _dal.Call.ReadAll().FirstOrDefault(c => c.CallId == callId);
            if (call == null)
                throw new BO.BlDoesNotExistException($"Call with ID {callId} does not exist or could not be found in the database.");

            DO.Volunteer? volunteer = _dal.Volunteer.ReadAll().FirstOrDefault(v => v.VolunteerId == volId);
            if (volunteer == null)
                throw new BO.BlDoesNotExistException($"Volunteer with ID {volId} does not exist or could not be found in the database.");

            // בדיקה אם הקריאה והמתנדב חוקיים
            BO.Call boCall = Helpers.CallManager.ConvertCallToBO(call, _dal);

            // בדיקת הקריאה
            if (!Helpers.CallManager.CheckCall(boCall))
                throw new BO.BlInvalidValueException($"Call with ID {callId} is invalid due to incorrect data.");

            // המרת המתנדב מ-DO ל-BO
            BO.Volunteer boVolunteer = Helpers.VolunteerManager.ConvertVolToBO(volunteer);

            // בדיקת המתנדב
            if (!Helpers.VolunteerManager.CheckVolunteer(boVolunteer))
                throw new BO.BlInvalidValueException($"Volunteer with ID {volId} is invalid due to incorrect data.");

            // שליפת רשימת ההקצאות משכבת הנתונים
            IEnumerable<DO.Assignment> assignments = _dal.Assignment.ReadAll();

            // בדיקה אם הקריאה כבר טופלה
            bool isCallAlreadyHandled = assignments.Any(a => a.CallId == callId && a.FinishTime != null);
            if (isCallAlreadyHandled)
                throw new BO.BlInvalidValueException($"Call with ID {callId} has already been handled and cannot be reassigned.");

            // בדיקה אם הקריאה נמצאת בטיפול פעיל
            bool isCallInProcess = assignments.Any(a => a.CallId == callId && a.FinishTime == null);
            if (isCallInProcess)
                throw new BO.BlInvalidValueException($"Call with ID {callId} is currently being handled by another volunteer.");

            // יצירת ישות הקצאה חדשה
            var newAssignment = new DO.Assignment
            (
                Id: assignments.Any()
                    ? assignments.Max(a => a.Id) + 1
                    : 1, // יצירת מזהה ייחודי
                CallId: callId,
                VolunteerId: volId,
                StartTime: DateTime.Now,
                TypeOfEnd: null, // ערך ברירת מחדל
                FinishTime: null // ערך ברירת מחדל
            );

            // הוספת ההקצאה החדשה לשכבת הנתונים
            _dal.Assignment.Create(newAssignment);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            // טיפול בחריגת DL של אובייקט לא קיים
            throw new BO.BlDoesNotExistException(ex.Message);
        }
        catch (DO.DalInvalidValueException ex)
        {
            // טיפול בחריגת DL של נתונים לא חוקיים
            throw new BO.BlInvalidValueException(ex.Message);
        }
        catch (DO.DalArgumentNullException ex)
        {
            // טיפול בחריגת DL של אובייקט חסר
            throw new BO.BlArgumentNullException(ex.Message);
        }
        catch (Exception ex)
        {
            // טיפול בחריגות כלשהן שלא כוסו
            throw new Exception(ex.Message);
        }
    }




    public void Create(BO.Call c)
    {
        if (!Helpers.CallManager.CheckCall(c))
        {
            throw new BO.BlInvalidValueException("The call data provided is invalid. Please check the input and try again.");
        }
        try
        {
            _dal.Call.Create(new DO.Call
            {
                CallId = c.CallId,
                CallType = (DO.CallType)c.CallType,
                Address = c.Address,
                Latitude = c.Latitude,
                Longitude = c.Longitude,
                OpenTime = c.OpenTime,
                MaxTime = c.MaxTime,
                Details = c.Details,
            });
        }
        catch (DO.DalAlreadyExistException ex)
        {
            throw new BO.BlAlreadyExistException($"A call with ID {c.CallId} already exists. Please check the ID and try again.");
        }
    }


    public void Delete(int callId)
    {
        try
        {
            BO.Call? c = Read(callId);
            IEnumerable<DO.Assignment> assignments = _dal.Assignment.ReadAll();

            if (c.CallStatus != 0 || (c.callAssignInLists != null && c.callAssignInLists.Any(x => assignments.Any(a => a.VolunteerId == x.VolunteerId))))
                throw new BO.BlDeletionImpossible("Can't delete this Call.");

            if (!Helpers.CallManager.CheckCall(c))
            {
                throw new BO.BlInvalidValueException("Invalid Call data.");
            }

            // ניסיון למחוק את הקריאה
            _dal.Call.Delete(callId);
        }
        catch (DO.DalDeletionImpossible ex)
        {
            // טיפול בחריגה כשלא ניתן למחוק את הקריאה
            throw new BO.BlDeletionImpossible(ex.Message);
        }
        catch (DO.DalInvalidValueException ex)
        {
            // טיפול בחריגה של נתונים לא חוקיים
            throw new BO.BlInvalidValueException(ex.Message);
        }
        catch (Exception ex)
        {
            // טיפול בחריגות כלשהן שלא כוסו
            throw new BO.BlDoesNotExistException(ex.Message);
        }
    }

    public Call GetCallDetails(int callId)
    {
        try
        {
            BO.Call call = Read(callId);
            return call;
        }
        catch (Exception ex)
        {
            // טיפול בחריגות בלתי צפויות
            throw new BO.BlDoesNotExistException(ex.Message);
        }
    }

    public IEnumerable<BO.CallInList> GetListOfCalls(BO.CallInListSort? filterType = null, object? filterValue = null, BO.CallInListSort? sortType = null)
    {
        try
        {
            IEnumerable<DO.Call> calls = _dal.Call.ReadAll();
            IEnumerable<DO.Assignment> assignments = _dal.Assignment.ReadAll();

            if (filterType != null && filterValue != null)
            {
                calls = filterType switch
                {
                    BO.CallInListSort.Id => calls.Where(c => c.CallId == Convert.ToInt32(filterValue)),
                    BO.CallInListSort.CallType => calls.Where(c => c.CallType == (DO.CallType)filterValue),
                    BO.CallInListSort.OpenTime => calls.Where(c => c.OpenTime == (DateTime)filterValue),
                    BO.CallInListSort.TimeToEnd => calls.Where(c => c.MaxTime.HasValue && c.MaxTime.Value == (DateTime)filterValue),
                    _ => calls
                };
            }

            // יצירת רשימה של BO.CallInList
            var callInList = calls.Select(c => new BO.CallInList
            {
                CallId = c.CallId,
                CallType = (BO.CallType)c.CallType,
                OpenTime = c.OpenTime,
                LastName = null, // לא קיים שדה כזה במחלקת Call
                TimeToEnd = c.MaxTime.HasValue ? c.MaxTime.Value.Subtract(c.OpenTime) : (TimeSpan?)null,
                TimeToCare = c.MaxTime.HasValue ? c.MaxTime.Value.Subtract(DateTime.Now) : (TimeSpan?)null,
                CallInListStatus = (BO.CallInListStatus)Helpers.CallManager.GetCallStatus(c, assignments)
            }).ToList();

            // מיון הרשימה
            if (sortType != null)
            {
                callInList = sortType switch
                {
                    BO.CallInListSort.Id => callInList.OrderBy(c => c.CallId).ToList(),
                    BO.CallInListSort.CallType => callInList.OrderBy(c => c.CallType).ToList(),
                    BO.CallInListSort.OpenTime => callInList.OrderBy(c => c.OpenTime).ToList(),
                    BO.CallInListSort.TimeToEnd => callInList.OrderBy(c => c.TimeToEnd).ToList(),
                    BO.CallInListSort.TimeToCare => callInList.OrderBy(c => c.TimeToCare).ToList(),
                    BO.CallInListSort.CallInListStatus => callInList.OrderBy(c => c.CallInListStatus).ToList(),
                    _ => callInList.OrderBy(c => c.CallId).ToList()
                };
            }
            else
            {
                // מיון ברירת מחדל לפי CallId אם לא נבחר ערך
                callInList = callInList.OrderBy(c => c.CallId).ToList();
            }

            return callInList;
        }
        catch (Exception ex)
        {
            // טיפול בחריגות כלשהן שלא כוסו
            throw new BO.BlDoesNotExistException(ex.Message);
        }
    }



    /* public IEnumerable<BO.ClosedCallInList> GetListOfClosedCall(int volId, BO.CallType? type = null, BO.CloseCallInListSort? sort = null)
     {
         try
         {
             IEnumerable<DO.Assignment> assignment = _dal.Assignment.ReadAll();
             IEnumerable<DO.Call> calls = _dal.Call.ReadAll();

             assignment = assignment.Where(v => v.VolunteerId == volId);

             assignment = type switch
             {
                 BO.CallType.Math_Primary => assignment.Where(v => calls.FirstOrDefault(c => c.CallId == v.CallId)?.CallType == DO.CallType.Math_Primary).ToList(),
                 BO.CallType.Math_Middle => assignment.Where(v => calls.FirstOrDefault(c => c.CallId == v.CallId)?.CallType == DO.CallType.Math_Middle).ToList(),
                 BO.CallType.Math_High => assignment.Where(v => calls.FirstOrDefault(c => c.CallId == v.CallId)?.CallType == DO.CallType.Math_High).ToList(),
                 BO.CallType.English_Primary => assignment.Where(v => calls.FirstOrDefault(c => c.CallId == v.CallId)?.CallType == DO.CallType.English_Primary).ToList(),
                 BO.CallType.English_Middle => assignment.Where(v => calls.FirstOrDefault(c => c.CallId == v.CallId)?.CallType == DO.CallType.English_Middle).ToList(),
                 BO.CallType.English_High => assignment.Where(v => calls.FirstOrDefault(c => c.CallId == v.CallId)?.CallType == DO.CallType.English_High).ToList(),
                 BO.CallType.Grammary_Primary => assignment.Where(v => calls.FirstOrDefault(c => c.CallId == v.CallId)?.CallType == DO.CallType.Grammary_Primary).ToList(),
                 BO.CallType.Grammary_Middle => assignment.Where(v => calls.FirstOrDefault(c => c.CallId == v.CallId)?.CallType == DO.CallType.Grammary_Middle).ToList(),
                 BO.CallType.Grammary_High => assignment.Where(v => calls.FirstOrDefault(c => c.CallId == v.CallId)?.CallType == DO.CallType.Grammary_High).ToList(),
                 _ => assignment.ToList() 
             };

             if (sort != null)
             {
                 assignment = assignment.Where(v => v.TypeOfEnd == (DO.TypeOfEnd)sort).ToList();
             }

             var closedCallInList = assignment.Select(a => new BO.ClosedCallInList
             {
                 CallId = a.CallId,
                 CallType = (BO.CallType)calls.FirstOrDefault(c => c.CallId == a.CallId)?.CallType, 
                 TypeOfEnd = (BO.TypeOfEnd)a.TypeOfEnd 
             }).ToList();

             var result = sort switch
             {
                 BO.CloseCallInListSort.CallId => closedCallInList.OrderBy(v => v.CallId).ToList(),
                 BO.CloseCallInListSort.CallType => closedCallInList.OrderBy(v => v.CallType).ToList(),
                 BO.CloseCallInListSort.FinishTime => closedCallInList.OrderBy(v => v.FinishTime).ToList(),
                 BO.CloseCallInListSort.OpenTime => closedCallInList.OrderBy(v => v.OpenTime).ToList(),
                 BO.CloseCallInListSort.StartTime => closedCallInList.OrderBy(v => v.StartTime).ToList(),
                 BO.CloseCallInListSort.Adress => closedCallInList.OrderBy(v => v.Adress).ToList(),
                 BO.CloseCallInListSort.TypeOfEnd => closedCallInList.OrderBy(v => v.TypeOfEnd).ToList(),
                 _ => closedCallInList.OrderBy(v => v.CallId).ToList() 
             };
             return result;
         }
         catch (Exception ex)
         {
             throw new Exception(ex.Message);
         }
     }*/
    public IEnumerable<BO.ClosedCallInList> GetListOfClosedCall(int volId, BO.CallType? type = null, BO.CloseCallInListSort? sort = null)
    {
        try
        {
            var assignments = _dal.Assignment.ReadAll().Where(a => a.VolunteerId == volId).ToList();
            var calls = _dal.Call.ReadAll().ToList();

            // LINQ עם שימוש ב-let כדי לשמור על הקריאה המשויכת לכל הקצאה
            var assignmentWithCalls = from a in assignments
                                      let relatedCall = calls.FirstOrDefault(c => c.CallId == a.CallId)
                                      where relatedCall != null // התעלמות מהקצאות ללא קריאה משויכת
                                      select new
                                      {
                                          Assignment = a,
                                          RelatedCall = relatedCall
                                      };

            // סינון לפי סוג הקריאה אם נדרש
            if (type != null)
            {
                assignmentWithCalls = from ac in assignmentWithCalls
                                      where ac.RelatedCall.CallType == (DO.CallType)type
                                      select ac;
            }

            // המרה לרשימת קריאות סגורות
            var closedCallInList = from ac in assignmentWithCalls
                                   select new BO.ClosedCallInList
                                   {
                                       CallId = ac.Assignment.CallId,
                                       CallType = (BO.CallType)ac.RelatedCall.CallType,
                                       TypeOfEnd = (BO.TypeOfEnd)ac.Assignment.TypeOfEnd
                                   };

            // מיון הרשימה
            closedCallInList = sort switch
            {
                BO.CloseCallInListSort.CallId => closedCallInList.OrderBy(v => v.CallId),
                BO.CloseCallInListSort.CallType => closedCallInList.OrderBy(v => v.CallType),
                BO.CloseCallInListSort.TypeOfEnd => closedCallInList.OrderBy(v => v.TypeOfEnd),
                _ => closedCallInList.OrderBy(v => v.CallId) // מיון ברירת מחדל לפי CallId
            };

            return closedCallInList.ToList();
        }
        catch (Exception ex)
        {
            throw new BO.BlDoesNotExistException(ex.Message);
        }
    }

    public IEnumerable<BO.OpenCallInList> GetListOfOpenCall(int volId, BO.CallType? type = null, OpenCallInListSort? openCall = null)
    {
        try
        {
            IEnumerable<DO.Call> calls = _dal.Call.ReadAll();
            IEnumerable<DO.Assignment> assignments = _dal.Assignment.ReadAll();

            DO.Volunteer volunteer = _dal.Volunteer.Read(volId);

            BO.Volunteer boVolunteer = Helpers.VolunteerManager.ConvertVolToBO(volunteer);

            string volunteerAddress = boVolunteer.Adress;

            calls = calls.Where(call =>
            {
                var status = Helpers.CallManager.GetCallStatus(call, assignments);
                return status == BO.CallStatus.Open || status == BO.CallStatus.OpenAtRisk;
            });

            // סינון לפי סוג הקריאה אם type לא null
            if (type.HasValue)
            {
                calls = calls.Where(c => c.CallType == (DO.CallType)type);
            }

            // מיפוי הקריאות הפתוחות לישויות BO.OpenCallInList
            var openCalls = calls.Select(c => new BO.OpenCallInList
            {
                CallId = c.CallId,
                CallType = (BO.CallType)c.CallType,
                Address = c.Address,
                OpenTime = c.OpenTime,
                MaxTime = c.MaxTime,
                Details = c.Details,
                Distance = Helpers.Tools.DistanceCalculator.CalculateDistance(volunteerAddress, c.Address, boVolunteer.DistanceType)
            }).ToList();

            // מיון לפי openCall אם הוא לא null
            var result = openCall switch
            {
                OpenCallInListSort.CallId => openCalls.OrderBy(c => c.CallId).ToList(),
                OpenCallInListSort.CallType => openCalls.OrderBy(c => c.CallType).ToList(),
                OpenCallInListSort.Address => openCalls.OrderBy(c => c.Address).ToList(),
                OpenCallInListSort.OpenTime => openCalls.OrderBy(c => c.OpenTime).ToList(),
                OpenCallInListSort.MaxTime => openCalls.OrderBy(c => c.MaxTime).ToList(),
                OpenCallInListSort.Details => openCalls.OrderBy(c => c.Details).ToList(),
                OpenCallInListSort.Distance => openCalls.OrderBy(c => c.Distance).ToList(),
                _ => openCalls.OrderBy(c => c.CallId).ToList() // ברירת מחדל: מיון לפי CallId
            };

            return result;
        }
        catch (Exception ex)
        {
            throw new BO.BlDoesNotExistException(ex.Message);
        }
    }

    public Call? Read(int callId)
    {
        try
        {
            BO.Call call = Read(callId);

            if (call == null)
            {
                throw new BO.BlDoesNotExistException($"Call with ID {callId} was not found.");
            }

            return new BO.Call
            {
                CallId = call.CallId,
                CallType = call.CallType,
                Address = call.Address,
                OpenTime = call.OpenTime,
                MaxTime = call.MaxTime,
                CallStatus = call.CallStatus,
                Details = call.Details,
                Latitude = call.Latitude,
                Longitude = call.Longitude,
            };
        }
        catch (DO.DalDoesNotExistException ex)
        {
            // טיפול במצב של אובייקט לא קיים
            throw new BO.BlDoesNotExistException(ex.Message);
        }
        catch (Exception ex)
        {
            // טיפול בחריגות כלשהן שלא כוסו
            throw new BO.BlDoesNotExistException(ex.Message);
        }
    }


    public int[] SumOfCalls()
    {
        try
        {
            var calls = _dal.Call.ReadAll();
            var assignments = _dal.Assignment.ReadAll();

            int[] countOfCalls = calls
                .GroupBy(call => Helpers.CallManager.GetCallStatus(call, assignments))
                .Aggregate(
                    new int[Enum.GetValues(typeof(BO.CallStatus)).Length],
                    (arr, group) =>
                    {
                        arr[(int)group.Key] = group.Count();
                        return arr;
                    });

            return countOfCalls;
        }
        catch (Exception ex)
        {
            throw new BO.BlDoesNotExistException(ex.Message);
        }
    }

    public void Update(Call c)
    {
        try
        {
            if (!Helpers.CallManager.CheckCall(c))
            {
                throw new BO.BlInvalidValueException("Invalid call data");
            }
            else
            {
                try
                {
                    _dal.Call.Update(new DO.Call
                    {
                        CallId = c.CallId,
                        CallType = (DO.CallType)c.CallType,
                        Address = c.Address,
                        Latitude = c.Latitude,
                        Longitude = c.Longitude,
                        OpenTime = c.OpenTime,
                        MaxTime = c.MaxTime,
                        Details = c.Details,
                    });
                }
                catch (DO.DalAlreadyExistException ex)
                {
                    throw new BO.BlAlreadyExistException(ex.Message);
                }
            }
        }
        catch (DO.DalInvalidValueException ex)
        {
            throw new BO.BlInvalidValueException($"Invalid call data: {ex.Message}");
        }
        catch (Exception ex)
        {
            throw new BO.BlDoesNotExistException(ex.Message);
        }
    }

    public void UpdateCallCancelled(int id, int assiId)
    {
        try
        {
            var assignments = _dal.Assignment.ReadAll();
            var volunteers = _dal.Volunteer.ReadAll();

            var assignment = assignments.FirstOrDefault(a => a.Id == assiId);
            if (assignment == null)
            {
                throw new BO.BlArgumentNullException($"Assignment with ID {assiId} does not exist.");
            }

            var requester = volunteers.FirstOrDefault(v => v.VolunteerId == id);
            if (requester == null)
            {
                throw new BO.BlArgumentNullException($"Volunteer with ID {id} does not exist.");
            }

            bool isManager = volunteers.Any(v => v.VolunteerId == id && v.RoleType == DO.Role.Manager);
            bool isAssignedToVolunteer = assignment.VolunteerId == id;

            if (!isManager && !isAssignedToVolunteer)
            {
                throw new BO.BlInvalidValueException("Requester does not have permission to cancel this assignment.");
            }

            if (assignment.FinishTime != null)
            {
                throw new BO.BlInvalidValueException("Assignment has already been closed or expired.");
            }

            assignment = assignment with
            {
                FinishTime = DateTime.Now,
                TypeOfEnd = (DO.TypeOfEnd)((id == requester.VolunteerId)
                    ? DO.TypeOfEnd.CancelledByVolunteer
                    : DO.TypeOfEnd.CancelledByManager)
            };

            _dal.Assignment.Update(assignment);
        }
        catch (DO.DalArgumentNullException ex)
        {
            // טיפול בחריגת DL של אובייקט חסר
            throw new BO.BlArgumentNullException(ex.Message);
        }
        catch (DO.DalInvalidValueException ex)
        {
            // טיפול בחריגת DL של נתונים לא חוקיים
            throw new BO.BlInvalidValueException(ex.Message);
        }
 
        catch (Exception ex)
        {
            // טיפול בחריגות כלשהן שלא כוסו
            throw new BO.BlDoesNotExistException(ex.Message);
        }
    }


    public void UpdateCallFinished(int id, int assiId)
    {
        try
        {
            var assignments = _dal.Assignment.ReadAll();
            var volunteers = _dal.Volunteer.ReadAll();

            var assignment = assignments.FirstOrDefault(a => a.Id == assiId);
            if (assignment == null)
            {
                throw new BO.BlArgumentNullException($"Assignment with ID {assiId} does not exist.");
            }

            var requester = volunteers.FirstOrDefault(v => v.VolunteerId == id);
            if (requester == null)
            {
                throw new BO.BlArgumentNullException($"Volunteer with ID {id} does not exist.");
            }

            bool isAssignedToVolunteer = assignment.VolunteerId == id;

            if (!isAssignedToVolunteer)
            {
                throw new BO.BlInvalidValueException("Requester does not have permission to finish this assignment.");
            }

            if (assignment.FinishTime != null)
            {
                throw new DO.DalInvalidValueException("Assignment has already been closed or expired.");
            }

            assignment = assignment with
            {
                FinishTime = DateTime.Now,
                TypeOfEnd = (DO.TypeOfEnd)DO.TypeOfEnd.Fulfilled
            };

            _dal.Assignment.Update(assignment);
        }
        catch (DO.DalArgumentNullException ex)
        {
            // טיפול בחריגת DL של אובייקט חסר
            throw new BO.BlArgumentNullException(ex.Message);
        }
        catch (DO.DalInvalidValueException ex)
        {
            // טיפול בחריגת DL של נתונים לא חוקיים
            throw new BO.BlInvalidValueException(ex.Message);
        }

        catch (Exception ex)
        {
            // טיפול בחריגות כלשהן שלא כוסו
            throw new BO.BlDoesNotExistException(ex.Message);
        }
    }

}
