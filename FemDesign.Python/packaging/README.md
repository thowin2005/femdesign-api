

<a href="https://strusoft.com/software/3d-structural-analysis-software-fem-design/" target="_blank">
    <p align="center">
        <img src="assets/FD_logo.png" alt="isolated" width="200" style="centre"/>
    </p>
</a>

# Description

FEM-Design is an advanced and intuitive structural analysis software. We support all aspects of your structural engineering requirements: from 3D modelling, design and finite element analysis (FEA) of concrete, steel, timber, composite, masonry and foundation structures. All calculations are performed to Eurocode standards, with some specific National annexes.

The quick and easy nature of FEM-Design makes it an ideal choice for all types of construction tasks, from single element design to global stability analysis of large buildings, making it the best practical program for structural engineers to use for their day to day tasks.

## Example

```python
from femdesign.comunication import FemDesignConnection, Verbosity
from femdesign.calculate.command import DesignModule
from femdesign.calculate.analysis import Analysis, Design, CombSettings, CombItem


pipe = FemDesignConnection(fd_path= r"C:\Program Files\StruSoft\FEM-Design 23\fd3dstruct.exe",
                              minimized= False)
try:
    pipe.SetVerbosity(Verbosity.SCRIPT_LOG_LINES)
    pipe.Open(r"example/simple_beam.str")
    pipe.SetProjectDescription(project_name="Amazing project",
                            project_description="Created through Python",
                            designer="Marco Pellegrino Engineer",
                            signature="MP",
                            comment="Wish for the best",
                            additional_info={"italy": "amazing", "sweden": "amazing_too"})



    # Define combination settings
    combSettings = CombSettings()
    combSettings.combitems.append(CombItem.StaticAnalysis(NLE=True))

    static_analysis = Analysis.StaticAnalysis(combSettings)
    pipe.RunAnalysis(static_analysis)

    pipe.RunAnalysis(Analysis.FrequencyAnalysis(num_shapes=5))
    pipe.RunDesign(DesignModule.STEELDESIGN, Design(False))

    pipe.Save(r"example\to_delete\simple_beam_out_2.str")
    pipe.GenerateListTables(bsc_file=r"example\bsc\finite-elements-nodes.bsc",
                            csv_file=r"example\output\finite-elements-nodes.csv")
    pipe.GenerateListTables(bsc_file=r"example\bsc\quantity-estimation-steel.bsc",
                            csv_file=r"example\output\quantity-estimation-steel.csv")
    pipe.Exit()
except Exception as err:
    pipe.KillProgramIfExists()
    raise err
```

> [!NOTE]
> I want the readers to read it carefully as it contains many important docs.