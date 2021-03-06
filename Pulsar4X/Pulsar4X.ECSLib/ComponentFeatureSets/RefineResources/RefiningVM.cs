﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Pulsar4X.ECSLib
{
    public class RefiningVM : ViewModelBase, IDBViewmodel
    {
        Guid _factionGuid;
        RefiningDB _refineDB;
        StaticDataStore _staticData;
        IOrderHandler _orderHandler;
        int _pointsPerDay;
        public int PointsPerDay
        {
            get { return _pointsPerDay; }
            set
            {
                if (_pointsPerDay != value)
                {
                    _pointsPerDay = value;
                    OnPropertyChanged();
                }
            }
        }

        Dictionary<Guid, RefineJobVM> _currentJobsDict = new Dictionary<Guid, RefineJobVM>();
        public ObservableCollection<RefineJobVM> CurrentJobs { get; } = new ObservableCollection<RefineJobVM>();

        //public ObservableCollection<object> NewJobSelectionItems { get; } = new ObservableCollection<object>();
        public DictionaryVM<Guid, string> ItemDictionary { get; } = new DictionaryVM<Guid, string>(DisplayMode.Value);
        public int NewJobSelectedIndex { get { return ItemDictionary.SelectedIndex; } }
        public Guid NewJobSelectedItem { get { return ItemDictionary.GetKey(NewJobSelectedIndex); } }
        public ushort NewJobBatchCount { get; set; }
        public bool NewJobRepeat { get; set; }

        private ICommand _addNewJob;
        public ICommand AddNewJob
        {
            get
            {
                return _addNewJob ?? (_addNewJob = new CommandHandler(OnNewBatchJob, true));
            }
        }

        private CommandReferences _cmdRef;

        private void OnNewBatchJob()
        {
            DateTime dateTime = _refineDB.OwningEntity.Manager.ManagerSubpulses.SystemLocalDateTime;
            var newBatchCommand = new RefineOrdersCommand(_factionGuid, _refineDB.OwningEntity.Guid, dateTime, NewJobSelectedItem, NewJobBatchCount, NewJobRepeat);
            _orderHandler.HandleOrder(newBatchCommand);
            Update();
        }

        public RefiningVM(Game game, CommandReferences cmdRef, RefiningDB refiningDB)
        {
            _staticData = game.StaticData;
            _refineDB = refiningDB;
            _orderHandler = game.OrderHandler;
            _factionGuid = refiningDB.OwningEntity.GetDataBlob<OwnedDB>().OwnedByFaction.Guid;
            _cmdRef = cmdRef;
            foreach (var kvp in _staticData.ProcessedMaterials)
            {
                ItemDictionary.Add(kvp.Key, kvp.Value.Name);
            }
            ItemDictionary.SelectedIndex = 0;
            NewJobBatchCount = 1;
            NewJobRepeat = false;
        }

        public void Update()
        {
            PointsPerDay = _refineDB.PointsPerTick;

            /*
            foreach(var jobItem in _refineDB.JobBatchList)
            {

                if(!_currentJobsDict.ContainsKey(jobItem.JobID))
                {
                    var newJobVM = new RefineJobVM(this, _staticData, jobItem, _cmdRef);
                    _currentJobsDict.Add(jobItem.JobID, newJobVM);
                    CurrentJobs.Add(newJobVM);
                }
                _currentJobsDict[jobItem.JobID].Update();
            }*/

            for (int index = 0; index < _refineDB.JobBatchList.Count; index++)
            {
                var jobItem = _refineDB.JobBatchList[index];
                Guid jobID = jobItem.JobID;

                if (CurrentJobs.Count <= index)
                {
                    var newJobVM = new RefineJobVM(this, _staticData, jobItem, _cmdRef);
                    _currentJobsDict.Add(jobID, newJobVM);
                    CurrentJobs.Insert(index, newJobVM);
                }
                if(CurrentJobs[index].JobID != jobID)
                {
                    var outOfOrderVM = CurrentJobs[index];
                    CurrentJobs.Remove(outOfOrderVM);
                    if (_refineDB.JobBatchList.Contains(outOfOrderVM.JobItem))
                    {
                        int newIndex = _refineDB.JobBatchList.IndexOf(outOfOrderVM.JobItem);
                        CurrentJobs.Insert(newIndex, outOfOrderVM);
                    }
                    else
                    {
                        _currentJobsDict.Remove(jobID);
                    }

                    if (!_currentJobsDict.ContainsKey(jobID))
                    {
                        var newJobVM = new RefineJobVM(this, _staticData, jobItem, _cmdRef);
                        _currentJobsDict.Add(jobID, newJobVM);
                        CurrentJobs.Insert(index, newJobVM);
                    }
                }
                CurrentJobs[index].Update();
            }
        }
    }



    public class RefineJobVM : ViewModelBase
    {
        internal RefineingJob JobItem { get; private set; }
        StaticDataStore _staticData;
        internal Guid JobID { get { return JobItem.JobID; } }
        public CommandReferences _cmdRef;
        private IDBViewmodel _parent;
        public string Item { get; set; }
        public bool Repeat => JobItem.Auto;
        public int Completed => JobItem.NumberCompleted;
        public int BatchQuantity => JobItem.NumberOrdered;
        public int ProductionPointsLeft => JobItem.ProductionPointsLeft;
        public float ItemPercentRemaining { get; set; }
        internal RefineJobVM(RefiningVM parentVM, StaticDataStore staticData, RefineingJob job, CommandReferences cmdRef)
        {
            _parent = parentVM;
            _staticData = staticData;
            JobItem = job;
            Item = _staticData.ProcessedMaterials[JobItem.ItemGuid].Name;
            _cmdRef = cmdRef;
        }

        public ICommand ChangePriorityCmd { get { return new RelayCommand<short>(param => ChangePriority(param)); } }
        public void ChangePriority(short delta)
        {
            RePrioritizeCommand newCommand = new RePrioritizeCommand(_cmdRef.FactionGuid, _cmdRef.EntityGuid, _cmdRef.GetSystemDatetime, JobItem.JobID, delta);
            _cmdRef.Handler.HandleOrder(newCommand);
            _parent.Update();
        }

        internal void Update()
        {
            OnPropertyChanged(nameof(Repeat));
            OnPropertyChanged(nameof(Completed));
            OnPropertyChanged(nameof(BatchQuantity));
            OnPropertyChanged(nameof(ProductionPointsLeft));
            ItemPercentRemaining = (float)JobItem.NumberCompleted / JobItem.NumberOrdered  * 100;
            OnPropertyChanged(nameof(ItemPercentRemaining));
        }
    }
}