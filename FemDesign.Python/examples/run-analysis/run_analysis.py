from femdesign.comunication import FemDesignConnection, Verbosity
from femdesign.calculate.command import DesignModule
from femdesign.calculate.analysis import Analysis, Design, CombSettings, CombItem


pipe = FemDesignConnection(minimized= False)
try:
    pipe.SetVerbosity(Verbosity.SCRIPT_LOG_LINES)
    pipe.Open(r"simple_beam.str")
    pipe.SetProjectDescription(project_name="Amazing project",
                            project_description="Created through Python",
                            designer="Marco Pellegrino Engineer",
                            signature="MP",
                            comment="Wish for the best",
                            additional_info={"italy": "amazing", "sweden": "amazing_too"})



    static_analysis = Analysis.StaticAnalysis()
    pipe.RunAnalysis(static_analysis)

    freq_analysis = Analysis.FrequencyAnalysis(num_shapes=5)
    pipe.RunAnalysis(freq_analysis)

    pipe.RunDesign(DesignModule.STEELDESIGN, Design(False))

    pipe.Save(r"simple_beam_out_2.str")
    pipe.GenerateListTables(bsc_file=r"bsc\finite-elements-nodes.bsc",
                            csv_file=r"output\finite-elements-nodes.csv")
    pipe.GenerateListTables(bsc_file=r"bsc\quantity-estimation-steel.bsc",
                            csv_file=r"output\quantity-estimation-steel.csv")
    pipe.Exit()
except Exception as err:
    pipe.KillProgramIfExists()
    raise err