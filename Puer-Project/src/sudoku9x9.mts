//部署:npm run build
function intDiv(i: number, j: number): number {
    return Math.floor(i / j)
}

class Sudoku9x9{
    private bindTo: CS.Sudoku.Gameplay.Puzzle.Sudoku9x9;

    constructor(bindTo: CS.Sudoku.Gameplay.Puzzle.Sudoku9x9) {
        console.log("Sudoku9x9 TypeScript binding")
        this.bindTo = bindTo
        this.bindTo.jsGenerate = (i:number)=>this.generate(i);
        this.bindTo.jsValidate = ()=>this.validate();
    }

    generate(this: Sudoku9x9, emptyCount:number = 0) : void{
        // no way to call base.Generate() here, call in C#.
        //this.bindTo.Generate();   // would crash...
        this.fillDiagnonalBox();
        this.dfs(0,3);
        this.removeElements(emptyCount);
    }

    validate(this: Sudoku9x9):boolean{
        // I wonder why we dont have Array$1<number>.Length here...
        // Going down the naïve way's kina painful
        for(let i = 0; i<9; i++){
            for(let j = 0; j<9; j++){
                let item = this.bindTo.get_Item(this.getIndex(i,j));
                this.bindTo.set_Item(this.getIndex(i,j), 0)
                if(!this.checkIsNumberAvailable(i,j,item)){
                    return false;
                }
                this.bindTo.set_Item(this.getIndex(i,j), item)
            }
        }
        return true;
    }

    fillDiagnonalBox(this: Sudoku9x9) {
        let nums = Array.from({length: 9}, (_, i) => i + 1)
        for(let i = 0; i<3; i++){
            this.shuffle(nums)
            for(let j = 0; j<3; j++){
                for(let k = 0;k<3;k++){
                    this.bindTo.set_Item(this.getIndex(j+i*3, k+i*3), nums[j*3+k])
                }
            }
        }
    }

    dfs(this: Sudoku9x9, i: number, j: number): boolean {
        if (j >= 9) {
            if (i >= 8) return true;

            i++;
            j=0;
        }

        if (this.bindTo.get_Item(this.getIndex(i, j)) != 0) return this.dfs(i, j + 1)

        for (let num = 1; num <= 9; num++) {
            if (this.checkIsNumberAvailable(i, j, num)) {
                this.bindTo.set_Item(this.getIndex(i, j), num)
                if (this.dfs(i, j + 1)) return true
            }
        }

        this.bindTo.set_Item(this.getIndex(i, j), 0)
        return false
    }

    checkIsNumberAvailable(this:Sudoku9x9, i: number, j: number, num: number): boolean {
        for (let k = 0; k < 9; k++) {
            if (this.bindTo.get_Item(this.getIndex(i, k)) == num) return false
            if (this.bindTo.get_Item(this.getIndex(k, j)) == num) return false
            if (this.bindTo.get_Item(this.getIndex(i - i % 3 + intDiv(k, 3), j - j % 3 + k % 3)) == num) return false
        }
        return true;
    }

    removeElements(this: Sudoku9x9, emptyCount: number) {
        // getting lazy here, just generate a random array and remove the first n elements
        let nums = Array.from({length: 81}, (_, i) => i)
        this.shuffle(nums)
        for (let i = 0; i < emptyCount; i++) {
            this.bindTo.set_Item(nums[i], 0)
            this.bindTo.removedCellIndex.set_Item(i, nums[i]);
        }
    }

    getIndex(this:Sudoku9x9, i: number, j: number): number {
        return i * this.bindTo.sideLength + j
    }
    
    // Fisher-Yates shuffle
    shuffle(this: Sudoku9x9, arr:number[]): void {
        for (let i = arr.length - 1; i > 0; i--) {
            const j = Math.floor(Math.random() * (i + 1));
            [arr[i], arr[j]] = [arr[j], arr[i]];
        }
    }
}

export function init(bindTo: CS.Sudoku.Gameplay.Puzzle.Sudoku9x9) {
    new Sudoku9x9(bindTo);
}