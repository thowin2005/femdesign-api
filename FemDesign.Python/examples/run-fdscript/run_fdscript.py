from femdesign.comunication import FemDesignConnection, Verbosity
from femdesign.calculate.command import DesignModule, CmdOpen, CmdUser, CmdCalculation, CmdSave, CmdListGen, CmdProjDescr, User
from femdesign.calculate.analysis import Analysis, Design, CombSettings, CombItem
from femdesign.calculate.fdscript import Fdscript

pipe = FemDesignConnection(fd_path= r"C:\Program Files\StruSoft\FEM-Design 23\fd3dstruct.exe",
                              minimized= False)
try:
    log = r"example\x.log"
    cmd_open =  CmdOpen(r"example\simple_beam.str")
    
    cmd_resmode = CmdUser(User.RESMODE)

    comb = Comb()
    comb.combitems.append( CombItem.StaticAnalysis() )
    analysis = Analysis.StaticAnalysis(comb)
    cmd_analysis = CmdCalculation(analysis)

    design = Design(False)
    freq_analysis = Analysis.FrequencyAnalysis()
    cmd_freq = CmdCalculation(freq_analysis)

    cmd_save = CmdSave(r"example\simple_beam_out.str")

    cmd_list_gen = CmdListGen(r"example\nodal_displacement.bsc",
                              r"example\nodal_displacement.csv")

    cmd_project = CmdProjDescr("Test project", "Test project description", "Test designer", "Test signature",
                               "Test comment", {"a": "a_txt", "b": "b_txt"})


    fdscript = Fdscript(log, [cmd_open, cmd_project, cmd_resmode, cmd_analysis, cmd_freq, cmd_list_gen, cmd_save])
except Exception as err:
    pipe.KillProgramIfExists()
    raise err