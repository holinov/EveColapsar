using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Windows.Input;
using Zen.Eve.Colapsar.Annotations;

namespace Zen.Eve.Colapsar.Models
{
    public class WormholeModel : MvcModel
    {
        private string _class = "O477";
        private int _mass = 1000;
        private PassagesCollection _passages;

        public WormholeModel()
        {
            _passages = new PassagesCollection()
                {
                    new Passage()
                        {
                            Wormhole = this,
                            Number = -1,
                            PassedMass = 999
                        }
                };
        }

        private string _log;
        private int _currentJumpIndex;

        [NotNull]
        public string Class
        {
            get { return _class; }
            set
            {
                if (value == _class) return;
                _class = value;
                OnPropertyChanged();
            }
        }

        public int Mass
        {
            get { return _mass; }
            set
            {
                if (value == _mass) return;
                _mass = value;
                OnPropertyChanged();
                OnPropertyChanged("LeftMass");
            }
        }

        [NotNull]
        public PassagesCollection Passages
        {
            get { return _passages; }
            set
            {
                if (Equals(value, _passages)) return;
                _passages = value;
                OnPropertyChanged();
                OnPropertyChanged("LeftMass");
            }
        }

        public int LeftMass
        {
            get { return Mass - Passages.Sum(p => p.PassedMass); }
        }

        public BuildPathCommand BuildPath
        {
            get { return new BuildPathCommand(this); }
        }

        public string Log
        {
            get { return _log; }
            set
            {
                if (value == _log) return;
                _log = value;
                OnPropertyChanged();
            }
        }
        public int CurrentJumpIndex
        {
            get { return _currentJumpIndex; }
            set
            {
                if (value == _currentJumpIndex) return;
                _currentJumpIndex = value;
                OnPropertyChanged();
            }
        }

        public MakeJumpCommand MakeJump
        {
            get { return new MakeJumpCommand(this); }
        }
        public UndoJumpCommand UndoJump
        {
            get { return new UndoJumpCommand(this); }
        }

        public void AddPassage(int passedMass, bool mwdUsed)
        {
            var direction = Passages.Count%2 == 0 ? PassageDirection.Out : PassageDirection.In;
            Passages.Add(new Passage()
                {
                    PassedMass = passedMass,
                    UsedMwd = mwdUsed,
                    PassageTime = DateTime.Now,
                    Wormhole = this,
                    Number = Passages.Count + 1,
                    Direction = direction
                });
            OnPropertyChanged("LeftMass");
            OnPropertyChanged("Passages");
        }

        #region Commands

        public class MakeJumpCommand:ICommand
        {
            private readonly WormholeModel _wh;

            public MakeJumpCommand(WormholeModel wh)
            {
                _wh = wh;
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                if (_wh.CurrentJumpIndex < _wh.Passages.Count - 1)
                {
                    _wh.CurrentJumpIndex++;
                    var curPassage = _wh.Passages[_wh.CurrentJumpIndex];
                    curPassage.JumpExecuted = true;
                    curPassage.PassageTime = DateTime.UtcNow;
                }
            }

            public event EventHandler CanExecuteChanged;
        }
        public class UndoJumpCommand : ICommand
        {
            private readonly WormholeModel _wh;

            public UndoJumpCommand(WormholeModel wh)
            {
                _wh = wh;
            }

            public bool CanExecute(object parameter)
            {
                return true;

            }

            public void Execute(object parameter)
            {
                if (_wh.CurrentJumpIndex > -1)
                {
                    var curPassage = _wh.Passages[_wh.CurrentJumpIndex];
                    curPassage.JumpExecuted = false;
                    _wh.CurrentJumpIndex--;
                }
            }

            public event EventHandler CanExecuteChanged;
        }

        public class BuildPathCommand : ICommand
        {
            private readonly WormholeModel _wormholeModel;

            public BuildPathCommand(WormholeModel wormholeModel)
            {
                _wormholeModel = wormholeModel;
            }

            public bool CanExecute(object parameter)
            {
                return _wormholeModel != null;
            }

            public void Execute(object parameter)
            {
                _wormholeModel.Passages.Clear();
                var res = BuildTree(_wormholeModel.Mass, true).ToArray();
                Console.WriteLine(res.Count());
                //те решения которые схлопнули дырку при прыжке внутрь
                var solutions = res.Where(s => s.MassLeft < 0 && s.Direction == PassageDirection.In && s.Mvd);

                var best = solutions.OrderBy(s => s.GetPath().Count()).FirstOrDefault();
                if (best != null)
                    foreach (var possibleSolution in best.GetPath())
                    {
                        _wormholeModel.AddPassage(new Passage()
                            {
                                Wormhole = _wormholeModel,
                                Direction = possibleSolution.Direction,
                                Number = possibleSolution.JumpNumber,
                                PassedMass = possibleSolution.JumpMass,
                                UsedMwd = possibleSolution.Mvd,
                                JumpExecuted = false,
                                MassLeft = possibleSolution.MassLeft
                            });
                    }

                //BuildByRules();
                //BuildRecurcive();
                var sb = new StringBuilder();
                sb.AppendFormat("Рассчет прыжков для чревоточины {0} рассчетная масса {1}", _wormholeModel.Class,
                                _wormholeModel.Mass)
                  .AppendLine();

                for (int index = 0; index < _wormholeModel.Passages.Count; index++)
                {
                    var passage = _wormholeModel.Passages[index];
                    sb.AppendFormat("{0}) {3} {1} масса: {2}",
                                    index,
                                    passage.UsedMwd ? "c MVD" : "БЕЗ MVD",
                                    passage.PassedMass, index%2 == 0 ? "туда" : "обратно")
                      .AppendLine();
                }
                _wormholeModel.Log = sb.ToString();
            }

            private void TreeBuild()
            {
                //Построить дерево всех возможных решений где последние прыжок будет наружу и масса будет < 0
            }

            private class PossibleSolution
            {
                public int JumpMass { get; set; }

                public PassageDirection Direction { get; set; }

                public bool Mvd { get; set; }

                public int MassLeft { get; set; }

                public int JumpNumber { get; set; }

                public PossibleSolution Parent { get; set; }
            
                public IEnumerable<PossibleSolution> GetPath()
                {
                    if (Parent == null)
                    {
                        yield return this;
                        yield break;
                    }
                    foreach (var parent in Parent.GetPath())
                    {
                        yield return parent;
                    }
                    yield return this;

                }
            }
            private IEnumerable<PossibleSolution> BuildTree(int mass,bool jumpOut,bool mwd=true,int lvl=0,PossibleSolution parent=null)
            {
                if (mass <= 0) yield break;
                
                var jumpMass = mwd ? 150 : 100;
                var direction = jumpOut ? PassageDirection.Out : PassageDirection.In;
                var thisSolution = new PossibleSolution()
                    {
                        JumpMass = jumpMass,
                        Direction = direction,
                        Mvd = mwd,
                        MassLeft = mass - jumpMass,
                        JumpNumber = lvl + 1,
                        Parent = parent
                    };

                yield return  thisSolution;

                foreach (var possibleSolution in BuildTree(mass-jumpMass,!jumpOut,mwd,lvl+1,thisSolution))
                {
                    yield return possibleSolution;
                    if(possibleSolution.MassLeft < 0 && possibleSolution.Direction==PassageDirection.In && possibleSolution.Mvd)
                        yield break;
                }
                foreach (var possibleSolution in BuildTree(mass - jumpMass, !jumpOut, !mwd, lvl + 1,thisSolution))
                {
                    yield return possibleSolution;
                    if (possibleSolution.MassLeft < 0 && possibleSolution.Direction == PassageDirection.In && possibleSolution.Mvd)
                        yield break;
                }
            } 

            /*private void BuildRecurcive()
            {

                var resulList = new List<Passage>(_wormholeModel.Mass / 300);
                var foundSolution = false;

                var curmdw = true;
                var curDirection = true;
                ulong idx = 0;
                while (!foundSolution)
                {
                    PassageDirection direction = curDirection ? PassageDirection.Out : PassageDirection.In;
                    var pass = new Passage()
                    {
                        UsedMwd = curmdw,
                        PassedMass = curmdw ? 150 : 100,
                        Wormhole = _wormholeModel,
                        Number = _wormholeModel.Passages.Count + 1,
                        Direction = direction,
                    };
                    resulList.Add(pass);

                    curDirection = idx%2 == 0;


                    //если добаленное закрыло и остались внутри - прервать
                    foundSolution = resulList.Last().Direction == PassageDirection.In &&
                                    _wormholeModel.Mass <= resulList.Sum(p => p.PassedMass);
                    idx++;
                }


                /*var resulList1 = new List<Passage>(_wormholeModel.Mass / 300);
                var found1=InnerRecursiveBuild(true, false, resulList1);
                var resulList2 = new List<Passage>(_wormholeModel.Mass / 300);
                var found2=InnerRecursiveBuild(true, false, resulList2);

                var resList = new List<Passage>();
                if (found1)
                    resList = resulList1;
                else if (found2)
                    resList = resulList2;
                _wormholeModel.Passages=new PassagesCollection(resList);
            }*/

            /*private bool InnerRecursiveBuild(bool mwd,bool isin,List<Passage> acc)
            {
                if (acc == null)
                    acc = new List<Passage>();

                var pass = new Passage()
                    {
                        UsedMwd = mwd,
                        PassedMass = mwd ? 150 : 100,
                        Wormhole = _wormholeModel,
                        Number = _wormholeModel.Passages.Count + 1,
                        Direction = isin ? PassageDirection.In : PassageDirection.Out,
                    };

                //Если схлопнулось
                if (_wormholeModel.Mass <= 0)
                {
                    //Если выходим вернуть да
                    return pass.Direction == PassageDirection.Out;
                }
                else
                {
                    acc.Add(pass);
                    if(acc.Sum(p=>p.PassedMass))
                    return childres;
                }
            }*/

            /*private void BuildByRules()
            {
                _wormholeModel.Passages.Clear();
                _wormholeModel.CurrentJumpIndex = -1;
                while (_wormholeModel.LeftMass > 150)
                {
                    if (_wormholeModel.LeftMass >= 500)
                    {
                        _wormholeModel.AddPassage(150, true);
                    }
                    else if (_wormholeModel.LeftMass <= 250)
                    {
                        _wormholeModel.AddPassage(100, false);
                        _wormholeModel.AddPassage(150, true);
                    }
                    else
                    {
                        _wormholeModel.AddPassage(100, false);
                    }
                }
            }*/

            public event EventHandler CanExecuteChanged;
        }

        private void AddPassage(Passage passedMass)
        {
            Passages.Add(passedMass);
            OnPropertyChanged("LeftMass");
        }

        #endregion
    }
}
