﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FemDesign.GenericClasses;
using System.Xml.Serialization;
using FemDesign.Calculate;

namespace FemDesign.Results
{
    public enum ResultType
    {
        #region QUANTITY ESTIMATION
        /* QUANTITY ESTIMATION */
        /// <summary>
        /// Quantity estimation, Concrete
        /// </summary>
        [Parseable("QuantityEstimationConcrete")]
        [Result(typeof(QuantityEstimationConcrete), ListProc.QuantityEstimationConcrete)]
        QuantityEstimationConcrete,

        /// <summary>
        /// Quantity estimation, Steel
        /// </summary>
        [Parseable("QuantityEstimationSteel")]
        [Result(typeof(QuantityEstimationSteel), ListProc.QuantityEstimationSteel)]
        QuantityEstimationSteel,

        /// <summary>
        /// Quantity estimation, Timber
        /// </summary>
        [Parseable("QuantityEstimationTimber")]
        [Result(typeof(QuantityEstimationTimber), ListProc.QuantityEstimationTimber)]
        QuantityEstimationTimber,

        /// <summary>
        /// Quantity estimation, Timber panel
        /// </summary>
        [Parseable("QuantityEstimationTimberPanel")]
        [Result(typeof(QuantityEstimationTimberPanel), ListProc.QuantityEstimationTimberPanel)]
        QuantityEstimationTimberPanel,

        /// <summary>
        /// Quantity estimation, Reinforcement
        /// </summary>
        [Parseable("QuantityEstimationReinforcement")]
        [Result(typeof(QuantityEstimationReinforcement), ListProc.QuantityEstimationReinforcement)]
        QuantityEstimationReinforcement,

        /// <summary>
        /// Quantity estimation, Profiled panel
        /// </summary>
        [Parseable("QuantityEstimationProfiledPanel")]
        [Result(typeof(QuantityEstimationProfiledPlate), ListProc.QuantityEstimationProfiledPanel)]
        QuantityEstimationProfiledPanel,

        /// <summary>
        /// Quantity estimation, Masonry
        /// </summary>
        [Parseable("QuantityEstimationMasonry")]
        [Result(typeof(QuantityEstimationMasonry), ListProc.QuantityEstimationMasonry)]
        QuantityEstimationMasonry,
        #endregion

        #region LOAD CASES AND COMBINATIONS
        /* LOAD CASES AND COMBINATIONS */
        /// <summary>
        /// Node, Displacements
        /// </summary>
        [Parseable("NodalDisplacement")]
        [Result(typeof(NodalDisplacement), ListProc.NodalDisplacementsLoadCase, ListProc.NodalDisplacementsLoadCombination)]
        NodalDisplacement,

        /// <summary>
        /// Bars, Internal Forces
        /// </summary>
        [Parseable("BarInternalForces")]
        [Result(typeof(BarInternalForce), ListProc.BarsInternalForcesLoadCase, ListProc.BarsInternalForcesLoadCombination)]
        BarInternalForce,

        /// <summary>
        /// Bars, Internal Forces
        /// </summary>
        [Parseable("BarStresses")]
        [Result(typeof(BarStress), ListProc.BarsStressesLoadCase, ListProc.BarsStressesLoadCombination)]
        BarStress,

        /// <summary>
        /// Bars, Displacements
        /// </summary>
        [Parseable("BarDisplacement")]
        [Result(typeof(BarDisplacement), ListProc.BarsDisplacementsLoadCase, ListProc.BarsDisplacementsLoadCombination)]
        BarDisplacement,

        /// <summary>
        /// Point support group, Reactions
        /// </summary>
        [Parseable("PointSupportReaction")]
        [Result(typeof(PointSupportReaction), ListProc.PointSupportReactionsLoadCase, ListProc.PointSupportReactionsLoadCombination)]
        PointSupportReaction,

        /// <summary>
        /// Line support group, Reactions
        /// </summary>
        [Parseable("LineSupportReaction")]
        [Result(typeof(LineSupportReaction), ListProc.LineSupportReactionsLoadCase, ListProc.LineSupportReactionsLoadCombination)]
        LineSupportReaction,

        /// <summary>
        /// Line support group, Resultants
        /// </summary>
        [Parseable("LineSupportResultant")]
        [Result(typeof(LineSupportResultant), ListProc.LineSupportResultantsLoadCase, ListProc.LineSupportResultantsLoadCombination)]
        LineSupportResultant,

        /// <summary>
        /// Shells, Internal Force
        /// </summary>
        [Parseable("ShellInternalForce")]
        [Result(typeof(ShellInternalForce), ListProc.ShellInternalForceLoadCase, ListProc.ShellInternalForceLoadCombination)]
        ShellInternalForce,

        /// <summary>
        /// Shells, Internal Force (Extract)
        /// </summary>
        [Parseable("ShellInternalForceExtract")]
        [Result(typeof(ShellInternalForce), ListProc.ShellInternalForceExtractLoadCase, ListProc.ShellInternalForceExtractLoadCombination)]
        ShellInternalForceExtract,

        /// <summary>
        /// Shells, Derived Forces
        /// </summary>
        [Parseable("ShellDerivedForces")]
        [Result(typeof(ShellDerivedForce), ListProc.ShellDerivedForceLoadCase, ListProc.ShellDerivedForceLoadCombination)]
        ShellDerivedForce,

        /// <summary>
        /// Shells, Derived Forces
        /// </summary>
        [Parseable("ShellDerivedForcesExtract")]
        [Result(typeof(ShellDerivedForce), ListProc.ShellDerivedForceExtractLoadCase, ListProc.ShellDerivedForceExtractLoadCombination)]
        ShellDerivedForceExtract,

        /// <summary>
        /// Shells, Stress
        /// </summary>
        [Parseable("ShellStress")]
        [Result(typeof(ShellStress), ListProc.ShellStressesTopLoadCase, ListProc.ShellStressesTopLoadCombination, ListProc.ShellStressesMembraneLoadCase, ListProc.ShellStressesMembraneLoadCombination, ListProc.ShellStressesBottomLoadCase, ListProc.ShellStressesBottomLoadCombination)]
        ShellStress,

        /// <summary>
        /// Shells, Stress (Extract)
        /// </summary>
        [Parseable("ShellStressExtract")]
        [Result(typeof(ShellStress), ListProc.ShellStressesTopExtractLoadCase, ListProc.ShellStressesTopExtractLoadCombination, ListProc.ShellStressesMembraneExtractLoadCase, ListProc.ShellStressesMembraneExtractLoadCombination, ListProc.ShellStressesBottomExtractLoadCase, ListProc.ShellStressesBottomExtractLoadCombination)]
        ShellStressExtract,

        /// <summary>
        /// Shells, Displacements (Extract)
        /// </summary>
        [Parseable("ShellDisplacementExtract")]
        [Result(typeof(ShellsDisplacement), ListProc.ShellDisplacementExtractLoadCase, ListProc.ShellDisplacementExtractLoadCombination)]
        ShellDisplacementExtract,
        #endregion

        #region EIGEN FREQUENCIES
        /// <summary>
        /// Eigen Frequencies: Structural Model
        /// </summary>
        [Parseable("EigenFrequencies")]
        [Result(typeof(EigenFrequencies), ListProc.EigenFrequencies)]
        EigenFrequencies,
        #endregion

        #region FOOTFALL
        /// <summary>
        /// Nodal Response Factor: FootFall
        /// </summary>
        [Parseable("NodalResponseFactor")]
        [Result(typeof(NodalResponseFactor), ListProc.NodalResponseFactor)]
        NodalResponseFactor,

        /// <summary>
        /// Nodal Acceleration: FootFall
        /// </summary>
        [Parseable("NodalAcceleration")]
        [Result(typeof(NodalAcceleration), ListProc.NodalAcceleration)]
        NodalAcceleration,

        #endregion

        #region RC design
        /* RC design */
        /// <summary>
        /// Shell, Utilization
        /// </summary>
        [Parseable("RCDesignShellUtilization")]
        [Result(typeof(ShellsDisplacement), ListProc.RCDesignShellUtilizationLoadCombination)]
        RCDesignShellUtilization,

        /// <summary>
        /// Shell, Crack width
        /// </summary>
        [Parseable("RCDesignShellCrackWidth")]
        [Result(typeof(ShellsDisplacement), ListProc.RCDesignShellCrackWidthLoadCombination)]
        RCDesignShellCracking,
        #endregion
    }
}
